import type {ReactNode} from 'react';
import clsx from 'clsx';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import Heading from '@theme/Heading';

import styles from './index.module.css';

function HomepageHeader() {
  const {siteConfig} = useDocusaurusContext();
  return (
    <header className={clsx('hero hero--primary', styles.heroBanner)}>
      <div className="container">
        <Heading as="h1" className="hero__title">
          {siteConfig.title}
        </Heading>
        <p className="hero__subtitle">{siteConfig.tagline}</p>
        <div className={styles.buttons}>
          <Link className="button button--secondary button--lg" to="/docs/intro">
            阅读文档
          </Link>
          <Link
            className="button button--outline button--secondary button--lg"
            style={{marginLeft: '1rem'}}
            to="/docs/core/quick-start">
            Core 快速开始
          </Link>
        </div>
      </div>
    </header>
  );
}

const features = [
  {
    title: '统一地基',
    description: '二进制、CRC、帧缓冲、传输与 Result 模型一次做对，协议包可组合。',
  },
  {
    title: '生产级质量',
    description: 'netstandard2.0 + net8.0、单元测试、SourceLink、语义化版本与 CHANGELOG。',
  },
  {
    title: '工业场景友好',
    description: '可预期失败用 CommResult；超时、重试、诊断事件开箱可用。',
  },
];

export default function Home(): ReactNode {
  const {siteConfig} = useDocusaurusContext();
  return (
    <Layout title={siteConfig.title} description={siteConfig.tagline}>
      <HomepageHeader />
      <main>
        <section className={styles.features}>
          <div className="container">
            <div className="row">
              {features.map((f) => (
                <div key={f.title} className={clsx('col col--4')}>
                  <div className="text--center padding-horiz--md">
                    <Heading as="h3">{f.title}</Heading>
                    <p>{f.description}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </section>
      </main>
    </Layout>
  );
}
