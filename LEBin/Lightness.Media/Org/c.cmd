@echo off
csc /t:library /out:Lightness.Media.dll /r:..\DFrameworkCL.dll /r:..\Lightness.Core.dll *.cs  && move /y *.dll ..

