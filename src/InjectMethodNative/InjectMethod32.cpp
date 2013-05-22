// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#include "stdafx.h"

#include "InjectMethod32.h"
#include <vcclr.h>

using namespace System;
using namespace Runtime::InteropServices;

static unsigned int WM_INJECTCODE = ::RegisterWindowMessage(L"WM_INJECTCODE!");
static HHOOK _messageHookHandle;


extern "C" __declspec(dllexport)  LPCSTR __stdcall InjectCode(HWND windowHandle, LPCSTR assembly, LPCSTR className, LPCSTR methodName, LPCSTR methodParam)
{
	String^ assemblyName = gcnew String(assembly);
	String^ classNameString = gcnew String(className);
	String^ methodNameString = gcnew String(methodName);
	String^ methodParamString = gcnew String(methodParam);

	Text::StringBuilder^ logBuilder = gcnew Text::StringBuilder();

	String^ assemblyClassAndMethod = assemblyName + "|" + classNameString + "|" + methodNameString + "|" + methodParamString;
	logBuilder->AppendLine(String::Format("Assembly class and method: {0}", assemblyClassAndMethod));

	HINSTANCE hInstDll;	

	if (::GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS, (LPCTSTR)&GlobalMessageHook, &hInstDll))
	{
		logBuilder->AppendFormat("Got global hook handle\r\n");

		DWORD processId = 0;
		DWORD threadId = ::GetWindowThreadProcessId(windowHandle, &processId);

		if (processId)
		{
			logBuilder->AppendFormat("processId: {0}\r\n", processId);
			logBuilder->AppendFormat("threadId: {0}\r\n", threadId);

			HANDLE hProcess = ::OpenProcess(PROCESS_ALL_ACCESS, FALSE, processId);
			if (hProcess)
			{
				logBuilder->AppendFormat("Opened Process\r\n");

				int remoteBufferLength = (assemblyClassAndMethod->Length + 1) * sizeof(CHAR);
				void* assemblyClassAndMethodRemote = ::VirtualAllocEx(hProcess, NULL, remoteBufferLength, MEM_COMMIT, PAGE_READWRITE);

				if (assemblyClassAndMethodRemote)
				{
					logBuilder->AppendFormat("Allocated memory\r\n");

					LPCSTR assemblyClassAndMethodLocalBuffer = (LPCSTR) Marshal::StringToHGlobalAnsi(assemblyClassAndMethod).ToPointer();
					::WriteProcessMemory(hProcess, assemblyClassAndMethodRemote, assemblyClassAndMethodLocalBuffer, remoteBufferLength, NULL);
					delete assemblyClassAndMethodLocalBuffer;
				
					_messageHookHandle = ::SetWindowsHookEx(WH_CALLWNDPROC, &GlobalMessageHook, hInstDll, threadId);

					if (_messageHookHandle)
					{
						logBuilder->AppendFormat("Set message hook\r\n");

						::SendMessage(windowHandle, WM_INJECTCODE, (WPARAM)assemblyClassAndMethodRemote, 0);
						::UnhookWindowsHookEx(_messageHookHandle);

						logBuilder->Insert(0, "Success\r\n");
					}

					::VirtualFreeEx(hProcess, assemblyClassAndMethodRemote, 0, MEM_RELEASE);
				}

				::CloseHandle(hProcess);
			}
		}
		else
		{
			logBuilder->AppendFormat("Failed to get WindowProcessId: {0}\r\n", ::GetLastError());
		}

		::FreeLibrary(hInstDll);
	}
	else
	{
		logBuilder->AppendFormat("Error getting global hook method: {0}\r\n", ::GetLastError());
	}

	return (LPCSTR) Marshal::StringToHGlobalAnsi(logBuilder->ToString()).ToPointer();
}


extern "C" __declspec(dllexport)  LRESULT __stdcall GlobalMessageHook(int nCode, WPARAM wparam, LPARAM lparam)
{
	if (nCode == HC_ACTION)
	{
		CWPSTRUCT* msg = (CWPSTRUCT*)lparam;
		if (msg != NULL && msg->message == WM_INJECTCODE)
		{
			String^ assemblyAndMethodName = gcnew String((CHAR*)msg->wParam);
			cli::array<String^>^ assemblyAndMethodSplit = assemblyAndMethodName->Split('|', 4);

			if (assemblyAndMethodSplit->Length == 4)
			{
				String^ assemblyName = assemblyAndMethodSplit[0];
				String^ className = assemblyAndMethodSplit[1];
				String^ methodName = assemblyAndMethodSplit[2];
				String^ paramName = assemblyAndMethodSplit[3];

				Reflection::Assembly^ assembly = Reflection::Assembly::LoadFile(assemblyName);
				if (assembly != nullptr)
				{
					Type^ type = assembly->GetType(assemblyAndMethodSplit[1]);
					if (type != nullptr)
					{
						Reflection::MethodInfo^ methodInfo = type->GetMethod(methodName, Reflection::BindingFlags::Static | Reflection::BindingFlags::Public);
						if (methodInfo != nullptr)
						{
							// Default: Method that takes no parameters
							array<Object^>^ methodParams = nullptr;
							if (methodInfo->GetParameters()->Length == 1)
							{
								// Alternative: Allow for a single string parameter
								array<Object^>^ methodParams = gcnew array<Object^>(1);
								methodParams[0] = paramName;
							}

							methodInfo->Invoke(nullptr, methodParams);
						}
					}
				}
			}
		}
	}
	return CallNextHookEx(_messageHookHandle, nCode, wparam, lparam);
}