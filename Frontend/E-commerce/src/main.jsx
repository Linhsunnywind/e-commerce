import { createRoot } from 'react-dom/client'
import { ConfigProvider, App as AntApp } from 'antd'
import 'antd/dist/reset.css'
import './index.css'
import App from './App.jsx'

createRoot(document.getElementById('root')).render(
  <ConfigProvider theme={{ token: { colorPrimary: '#2563eb', borderRadius: 8, fontFamily: "'Segoe UI', system-ui, -apple-system, sans-serif" } }}>
    <AntApp>
      <App />
    </AntApp>
  </ConfigProvider>,
)
