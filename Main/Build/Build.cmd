set msbuild=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild

%msbuild% Packages.msbuild /v:minimal /maxcpucount /nodeReuse:false %*