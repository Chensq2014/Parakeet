# Parakeet
the latest net with abp framework and years of accumulation libraries

#### MySql 数据库迁移命令:

add-migration [MySqlMigrationName] -c MySqlMigrationsDbContext -o Migrations\MySqlMigrations
Script-Migration -From "[migration_pre文件名]" -To "[migration_next文件名]" -context MySqlMigrationsDbContext
update-database -c MySqlMigrationsDbContext

#### PgSql 数据库迁移命令:

add-migration [PgSqlMigrationName] -c PgSqlMigrationsDbContext -o Migrations\PgSqlMigrations
Script-Migration -From "[migration_pre文件名]" -To "[migration_next文件名]" -context PgSqlMigrationsDbContext
update-database -c PgSqlMigrationsDbContext

#### SqlServer 数据库迁移命令:

add-migration [SqlServerMigrationName] -c SqlServerMigrationsDbContext -o Migrations\SqlServerMigrations
Script-Migration -From "[migration_pre文件名]" -To "[migration_next文件名]" -context SqlServerMigrationsDbContext
update-database -c SqlServerMigrationsDbContext

#### Dotnet tool ef  安装ef命令行工具  cd到项目目录，
dotnet ef dbcontext list  ----查看当前启动项目 dbcontext有那些
dotnet ef migrations add Update3 -c xxxDbContext    ----添加指定DbContext的 migration迁移脚本
dotnet ef migrations script -c xxxDbContext  --经测试，只有这样写才可以，把全部的sql都script出来


# docker client 命令
docker system prune — 删除所有未使用的容器、网络以及无名称的镜像(虚悬镜像)

# 删除xxx image 不用的image需要手动删除
docker rmi xxx

# 移除所有parakeet服务
docker stack down parakeet

# 再根据yml文件创建/更新parakeet服务  可重复执行
docker stack deploy -c docker-compose-parakeet.yml parakeet

# 推送镜像到镜像仓库：
	Docker push 镜像名
	
# 先通过以上命令推送新镜像，本地环境docker pull 镜像名 拉取镜像，

# Nginx 更新配置
/etc/nginx/conf.d/xxx.xxx.com.conf
编辑这个文件，指定域名解析到的vpn对应本机的ip，
保存后执行reload配置文件命令即可【可以通过vim命令或者secureCRT可视化编辑】
vim /etc/nginx/conf.d/xxx.xxx.com.conf
nginx -s reload


