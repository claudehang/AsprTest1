# 数据库迁移脚本
Write-Host "开始应用数据库迁移..." -ForegroundColor Green

# 切换到Web项目目录
Set-Location -Path ".\AspireApp1.Web"

try {
    # 使用EF Core工具应用迁移
    # 注意：实际应用中，应考虑使用正式的EF Core迁移命令
    dotnet ef database update
    
    Write-Host "数据库迁移完成!" -ForegroundColor Green
}
catch {
    Write-Host "迁移过程中发生错误: $_" -ForegroundColor Red
}

# 回到原目录
Set-Location -Path ".."
