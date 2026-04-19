import { Link, Route, Routes } from 'react-router-dom';
import ProjectsPage from './pages/ProjectsPage';
import ProjectDetailPage from './pages/ProjectDetailPage';

export default function App() {
  const API_BASE = import.meta.env.VITE_API_BASE_URL;

  return (
    <div className="app">
      <header className="app-header">
        <Link to="/" className="brand">
          <span className="brand-mark">RT</span>
          <span>Requirements Tracker</span>
        </Link>
        <nav>
          <a href={`${API_BASE}/swagger`} target="_blank" rel="noreferrer">
            API Docs
          </a>
        </nav>
      </header>

      <main className="app-main">
        <Routes>
          <Route path="/" element={<ProjectsPage />} />
          <Route path="/projects/:id" element={<ProjectDetailPage />} />
        </Routes>
      </main>

      <footer className="app-footer">
        <span>Demo · .NET 8 · React · TypeScript · EF Core</span>
      </footer>
    </div>
  );
}