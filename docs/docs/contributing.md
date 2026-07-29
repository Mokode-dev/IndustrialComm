---
title: 贡献指南
---

# 贡献指南

## 开发环境

- .NET 8 SDK  
- Node.js 18+（文档站）  

## 构建

```bash
dotnet build IndustrialComm.sln -c Release
dotnet test IndustrialComm.sln -c Release
```

## 文档

```bash
cd docs
npm install
npm start
```

## 版本

遵循 [SemVer 2.0](https://semver.org/)。破坏性 API 变更需升级主版本并更新 CHANGELOG。

## 发布到 NuGet（Trusted Publishing）

推荐使用 [nuget.org Trusted Publishing](https://learn.microsoft.com/zh-cn/nuget/nuget-org/trusted-publishing)，**不需要**把长期 API Key 放进仓库。

1. 登录 [nuget.org](https://www.nuget.org) → 头像 → **Trusted Publishing** → 添加策略：
   - Repository owner：`Mokode-dev`
   - Repository：`IndustrialComm`
   - Workflow file：`publish-nuget.yml`（不要带路径）
2. GitHub 仓库 **Settings → Secrets and variables → Actions** 新增：
   - `NUGET_USER` = 你在 nuget.org 上的**用户名**（个人资料名，不是邮箱）
3. 改版本号（`Directory.Build.props` 的 `<Version>`）后打 tag 并推送：

```bash
git tag v1.0.0
git push origin v1.0.0
```

或在 GitHub **Actions → Publish NuGet → Run workflow** 手动运行。

工作流文件：`.github/workflows/publish-nuget.yml`。

## 许可证

贡献代码默认采用 MIT License。
