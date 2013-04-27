// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.


#pragma once


extern	"C" __declspec(dllexport)
LRESULT __stdcall GlobalMessageHook(int nCode, WPARAM wparam, LPARAM lparam);

extern "C" __declspec(dllexport)
LPCSTR __stdcall InjectCode(HWND windowHandle, LPCSTR assembly, LPCSTR className, LPCSTR methodName, LPCSTR methodParam);
