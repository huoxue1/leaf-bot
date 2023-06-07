# 设置变量
PROJECT_NAME = leaf-bot
TARGETS = \
    bin/Release/netcoreapp3.1/linux-x64/publish/$(PROJECT_NAME)-linux \
    bin/Release/netcoreapp3.1/win-x64/publish/$(PROJECT_NAME)-win.exe \
    bin/Release/netcoreapp3.1/osx-x64/publish/$(PROJECT_NAME)-osx

# 默认目标
all: build

# 编译项目
build:
	@for target in $(TARGETS); do \
		runtime=$$(echo $$target | sed 's/.*\/\(.*\)-.*$$/\1/'); \
		dotnet publish -c Release -r $$runtime --self-contained true -p:PublishSingleFile=true -o $$target $(PROJECT_NAME).csproj; \
	done

# 清理构建输出
clean:
	rm -rf $(TARGETS)

# 安装项目依赖
restore:
	dotnet restore $(PROJECT_NAME).csproj

# 运行项目
run:
	dotnet run --project $(PROJECT_NAME).csproj

.PHONY: all build clean restore run
