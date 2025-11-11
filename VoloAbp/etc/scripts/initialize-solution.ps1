abp install-libs

cd src/VoloAbp.OTel.DbMigrator && dotnet run && cd -



cd src/VoloAbp.OTel.Web && dotnet dev-certs https -v -ep openiddict.pfx -p config.auth_server_default_pass_phrase 


exit 0