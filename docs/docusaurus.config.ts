import {themes as prismThemes} from 'prism-react-renderer';
import type {Config} from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';

const config: Config = {
  title: 'IndustrialComm',
  tagline: '生产级 .NET 工业通信中间件生态',
  favicon: 'img/logo.png',

  future: {
    v4: true,
  },

  url: 'https://industrialcomm.github.io',
  baseUrl: '/',

  organizationName: 'Mokode-dev',
  projectName: 'IndustrialComm',

  onBrokenLinks: 'throw',

  i18n: {
    defaultLocale: 'zh-Hans',
    locales: ['zh-Hans'],
  },

  presets: [
    [
      'classic',
      {
        docs: {
          sidebarPath: './sidebars.ts',
          routeBasePath: 'docs',
          editUrl: 'https://github.com/Mokode-dev/IndustrialComm/tree/main/docs/',
        },
        blog: {
          showReadingTime: true,
          feedOptions: {
            type: ['rss', 'atom'],
            xslt: true,
          },
          onInlineTags: 'warn',
          onInlineAuthors: 'warn',
          onUntruncatedBlogPosts: 'ignore',
        },
        theme: {
          customCss: './src/css/custom.css',
        },
      } satisfies Preset.Options,
    ],
  ],

  themeConfig: {
    image: 'img/logo.png',
    colorMode: {
      respectPrefersColorScheme: true,
    },
    navbar: {
      title: 'IndustrialComm',
      logo: {
        alt: 'Indulink / IndustrialComm',
        src: 'img/logo.png',
      },
      items: [
        {
          type: 'docSidebar',
          sidebarId: 'docsSidebar',
          position: 'left',
          label: '文档',
        },
        {to: '/blog', label: '博客', position: 'left'},
        {
          href: 'https://github.com/Mokode-dev/IndustrialComm',
          label: 'GitHub',
          position: 'right',
        },
        {
          href: 'https://gitee.com/mokode/industrial-comm',
          label: 'Gitee',
          position: 'right',
        },
      ],
    },
    footer: {
      style: 'dark',
      links: [
        {
          title: '文档',
          items: [
            {label: '生态概览', to: '/docs/intro'},
            {label: 'Core 快速开始', to: '/docs/core/quick-start'},
          ],
        },
        {
          title: '包',
          items: [
            {label: 'Mokode.IndustrialComm.Core', to: '/docs/core/overview'},
            {label: '路线图', to: '/docs/roadmap'},
          ],
        },
        {
          title: '更多',
          items: [
            {label: 'GitHub', href: 'https://github.com/Mokode-dev/IndustrialComm'},
            {label: 'Gitee', href: 'https://gitee.com/mokode/industrial-comm'},
            {label: 'NuGet', href: 'https://www.nuget.org/packages/Mokode.IndustrialComm.Core'},
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} Mokode. MIT License.`,
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
      additionalLanguages: ['csharp', 'bash', 'json'],
    },
  } satisfies Preset.ThemeConfig,
};

export default config;
