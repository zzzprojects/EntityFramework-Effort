set msbuild=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild

%msbuild% Build.targets /v:minimal /maxcpucount /nodeReuse:false %*