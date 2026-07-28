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

## 许可证

贡献代码默认采用 MIT License。
