@echo off
csc /t:library /out:Lightness.Media.dll /r:..\DFramework.CSProxy.dll /r:..\Lightness.Core.dll *.cs  && move /y *.dll ..

