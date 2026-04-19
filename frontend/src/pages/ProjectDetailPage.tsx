import { useEffect, useMemo, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { api } from '../api/client';
import type {
  Project,
  Requirement,
  Priority,
  RequirementStatus,
  RequirementType
} from '../types/models';

const PRIORITY_ORDER: Priority[] = ['Must', 'Should', 'Could', 'Wont'];
const PRIORITY_LABEL: Record<Priority, string> = {
  Must: 'Must',
  Should: 'Should',
  Could: 'Could',
  Wont: "Won't"
};
const STATUS_LABEL: Record<RequirementStatus, string> = {
  Draft: 'Draft',
  Approved: 'Approved',
  InProgress: 'In Progress',
  Done: 'Done',
  Rejected: 'Rejected'
};
const STATUSES: RequirementStatus[] = ['Draft', 'Approved', 'InProgress', 'Done', 'Rejected'];

export default function ProjectDetailPage() {
  const { id } = useParams<{ id: string }>();
  const projectId = Number(id);

  const [project, setProject] = useState<Project | null>(null);
  const [requirements, setRequirements] = useState<Requirement[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // new-requirement form state
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [type, setType] = useState<RequirementType>('Functional');
  const [priority, setPriority] = useState<Priority>('Should');
  const [hours, setHours] = useState('');

  async function load() {
    setLoading(true);
    try {
      const [p, reqs] = await Promise.all([
        api.getProject(projectId),
        api.listRequirements(projectId)
      ]);
      setProject(p);
      setRequirements(reqs);
      setError(null);
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { if (projectId) load(); /* eslint-disable-next-line */ }, [projectId]);

  const totalHours = useMemo(
    () => requirements.reduce((sum, r) => sum + (r.estimatedHours ?? 0), 0),
    [requirements]
  );

  async function handleCreate(e: React.FormEvent) {
    e.preventDefault();
    try {
      await api.createRequirement(projectId, {
        title: title.trim(),
        description: description.trim() || null,
        type,
        priority,
        estimatedHours: hours ? Number(hours) : null
      });
      setTitle(''); setDescription(''); setHours('');
      setType('Functional'); setPriority('Should');
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function handleStatusChange(req: Requirement, status: RequirementStatus) {
    try {
      await api.updateRequirement(projectId, req.id, {
        title: req.title,
        description: req.description,
        type: req.type,
        priority: req.priority,
        status,
        estimatedHours: req.estimatedHours
      });
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function handleDelete(reqId: number) {
    if (!confirm('Delete this requirement?')) return;
    try {
      await api.deleteRequirement(projectId, reqId);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  if (loading) return <p>Loading project…</p>;
  if (!project) return <p>Project not found. <Link to="/">Go back</Link></p>;

  const grouped = PRIORITY_ORDER.map(p => ({
    priority: p,
    items: requirements.filter(r => r.priority === p)
  })).filter(g => g.items.length > 0);

  return (
    <section>
      <Link to="/" className="back-link">← All Projects</Link>

      <div className="page-header">
        <div>
          <h1>{project.name}</h1>
          <p className="subtitle">{project.customerName}</p>
        </div>
        <div className="stat-group">
          <div className="stat">
            <span className="stat-value">{requirements.length}</span>
            <span className="stat-label">Requirements</span>
          </div>
          <div className="stat">
            <span className="stat-value">{totalHours.toLocaleString('en-US')} h</span>
            <span className="stat-label">Estimated</span>
          </div>
        </div>
      </div>

      {project.description && <p className="project-desc-large">{project.description}</p>}

      {error && <div className="alert alert-error">{error}</div>}

      <div className="card form">
        <h2>New Requirement</h2>
        <form onSubmit={handleCreate}>
          <label>
            <span>Title</span>
            <input value={title} onChange={e => setTitle(e.target.value)} required maxLength={300} />
          </label>
          <label>
            <span>Description</span>
            <textarea value={description} onChange={e => setDescription(e.target.value)} rows={2} maxLength={4000} />
          </label>
          <div className="form-row">
            <label>
              <span>Type</span>
              <select value={type} onChange={e => setType(e.target.value as RequirementType)}>
                <option value="Functional">Functional</option>
                <option value="NonFunctional">Non-functional</option>
              </select>
            </label>
            <label>
              <span>Priority (MoSCoW)</span>
              <select value={priority} onChange={e => setPriority(e.target.value as Priority)}>
                {PRIORITY_ORDER.map(p => (
                  <option key={p} value={p}>{PRIORITY_LABEL[p]}</option>
                ))}
              </select>
            </label>
            <label>
              <span>Effort (h)</span>
              <input
                type="number" min="0" step="0.5"
                value={hours}
                onChange={e => setHours(e.target.value)}
              />
            </label>
          </div>
          <div className="form-actions">
            <button type="submit" className="btn btn-primary">Add</button>
          </div>
        </form>
      </div>

      {grouped.length === 0 ? (
        <div className="empty">No requirements added yet.</div>
      ) : (
        grouped.map(group => (
          <div key={group.priority} className="prio-group">
            <h3 className={`prio-heading prio-${group.priority.toLowerCase()}`}>
              {PRIORITY_LABEL[group.priority]} — {group.items.length}
            </h3>
            <ul className="card-list">
              {group.items.map(r => (
                <li key={r.id} className="card req-card">
                  <div className="req-body">
                    <div className="req-title-row">
                      <span className={`pill pill-${r.type.toLowerCase()}`}>
                        {r.type === 'Functional' ? 'Functional' : 'Non-functional'}
                      </span>
                      <strong>{r.title}</strong>
                      {r.estimatedHours != null && (
                        <span className="hours">{r.estimatedHours} h</span>
                      )}
                    </div>
                    {r.description && <p className="req-desc">{r.description}</p>}
                  </div>
                  <div className="req-actions">
                    <select
                      className={`status-select status-${r.status.toLowerCase()}`}
                      value={r.status}
                      onChange={e => handleStatusChange(r, e.target.value as RequirementStatus)}
                    >
                      {STATUSES.map(s => (
                        <option key={s} value={s}>{STATUS_LABEL[s]}</option>
                      ))}
                    </select>
                    <button className="btn btn-ghost" onClick={() => handleDelete(r.id)}>Delete</button>
                  </div>
                </li>
              ))}
            </ul>
          </div>
        ))
      )}
    </section>
  );
}
