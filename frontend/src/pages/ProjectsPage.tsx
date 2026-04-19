import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../api/client';
import type { Project } from '../types/models';

export default function ProjectsPage() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [name, setName] = useState('');
  const [customer, setCustomer] = useState('');
  const [description, setDescription] = useState('');

  async function load() {
    setLoading(true);
    try {
      setProjects(await api.listProjects());
      setError(null);
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { load(); }, []);

  async function handleCreate(e: React.FormEvent) {
    e.preventDefault();
    try {
      await api.createProject({
        name: name.trim(),
        customerName: customer.trim(),
        description: description.trim() || null
      });
      setName(''); setCustomer(''); setDescription(''); setShowForm(false);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function handleDelete(id: number) {
    if (!confirm('Delete project? All requirements will be lost.')) return;
    try {
      await api.deleteProject(id);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  return (
    <section>
      <div className="page-header">
        <div>
          <h1>Projects</h1>
          <p className="subtitle">All requirements specifications</p>
        </div>
        <button className="btn btn-primary" onClick={() => setShowForm(v => !v)}>
          {showForm ? 'Cancel' : '+ New Project'}
        </button>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {showForm && (
        <form className="card form" onSubmit={handleCreate}>
          <label>
            <span>Project name</span>
            <input value={name} onChange={e => setName(e.target.value)} required maxLength={200} />
          </label>
          <label>
            <span>Customer</span>
            <input value={customer} onChange={e => setCustomer(e.target.value)} required maxLength={200} />
          </label>
          <label>
            <span>Description</span>
            <textarea value={description} onChange={e => setDescription(e.target.value)} rows={3} maxLength={2000} />
          </label>
          <div className="form-actions">
            <button type="submit" className="btn btn-primary">Create</button>
          </div>
        </form>
      )}

      {loading ? (
        <p>Loading projects…</p>
      ) : projects.length === 0 ? (
        <div className="empty">No projects yet.</div>
      ) : (
        <ul className="card-list">
          {projects.map(p => (
            <li key={p.id} className="card project-card">
              <div className="project-card-body">
                <Link to={`/projects/${p.id}`} className="project-title">{p.name}</Link>
                <div className="project-meta">
                  <span>{p.customerName}</span>
                  <span>·</span>
                  <span>{p.requirementCount} requirements</span>
                </div>
                {p.description && <p className="project-desc">{p.description}</p>}
              </div>
              <button className="btn btn-ghost" onClick={() => handleDelete(p.id)}>Delete</button>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
