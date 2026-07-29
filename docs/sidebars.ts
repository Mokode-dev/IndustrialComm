import type {SidebarsConfig} from '@docusaurus/plugin-content-docs';

const sidebars: SidebarsConfig = {
  docsSidebar: [
    'intro',
    {
      type: 'category',
      label: 'Mokode.IndustrialComm.Core',
      collapsed: false,
      items: [
        'core/overview',
        'core/quick-start',
        'core/binary-and-checksum',
        'core/framing-and-transport',
        'core/results-and-reliability',
        'core/error-codes',
      ],
    },
    'roadmap',
    'contributing',
  ],
};

export default sidebars;
