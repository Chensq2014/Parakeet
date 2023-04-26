# Parakeet
the latest net with abp framework and years of accumulation libraries

#### PgSql 数据库迁移命令:

add-migration [PgSqlMigrationName] -c PgSqlMigrationsDbContext -o Migrations\PgSqlMigrations

update-database -c PgSqlMigrationsDbContext

#### SqlServer 数据库迁移命令:

add-migration [SqlServerMigrationName] -c SqlServerMigrationsDbContext -o Migrations\SqlServerMigrations

update-database -c SqlServerMigrationsDbContext

#### MySql 数据库迁移命令:

add-migration [MySqlMigrationName] -c MySqlMigrationsDbContext -o Migrations\MySqlMigrations

update-database -c MySqlMigrationsDbContext

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




# abp cli 常用命令
dotnet tool update -g Volo.Abp.Cli

abp new Parakeet.Net -dbms PostgreSQL --tiered -csf  --version 6.0.2

abp new Parakeet.Net -t app-nolayers -csf --version 6.0.2

abp new Parakeet.Net -t module -csf --version 6.0.2

abp new Parakeet.Net-t module --no-ui -csf --version 6.0.2

abp new Parakeet.Net -t app-nolayers --theme basic -csf --version 6.0.2


# 如果装错了，可以移除cli工具，再重新装指定版本
abp cli remove
dotnet tool install -g Volo.Abp.Cli --version 6.0.2

# mvc前端安装libs依赖命令 会自动把最小依赖安装包自动安装到 /wwwroot/libs 目录下

abp install-libs --version 6.0.2

或通过源码测试项目路径拷贝：
abp\framework\test\Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Demo\wwwroot\libs

