# 清理不必要的文件和文件夹
Write-Host "开始清理不必要的文件..." -ForegroundColor Green

# 删除bin和obj目录
Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | 
ForEach-Object {
    Write-Host "删除: $($_.FullName)" -ForegroundColor Yellow
    Remove-Item -Path $_.FullName -Recurse -Force
}

# 删除.vs目录
if (Test-Path -Path ".vs") {
    Write-Host "删除: .vs目录" -ForegroundColor Yellow
    Remove-Item -Path ".vs" -Recurse -Force
}

# 删除其他临时文件
Get-ChildItem -Path . -Include *.suo,*.user,*.userosscache,*.sln.docstates -Recurse -File | 
ForEach-Object {
    Write-Host "删除: $($_.FullName)" -ForegroundColor Yellow
    Remove-Item -Path $_.FullName -Force
}

Write-Host "清理完成！" -ForegroundColor Green
Write-Host "注意：已创建.gitignore文件，Git将来不会再跟踪这些文件。" -ForegroundColor Cyan
