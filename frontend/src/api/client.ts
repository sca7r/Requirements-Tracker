import type {
  Project,
  CreateProject,
  Requirement,
  CreateRequirement,
  UpdateRequirement
} from '../types/models';

const BASE = '/api';

async function request<T>(url: string, init?: RequestInit): Promise<T> {
  const res = await fetch(url, {
    headers: { 'Content-Type': 'application/json' },
    ...init
  });

  if (!res.ok) {
    const text = await res.text();
    throw new Error(`${res.status} ${res.statusText}: ${text}`);
  }

  if (res.status === 204) return undefined as T;
  return res.json() as Promise<T>;
}

export const api = {
  // Projects
  listProjects: () => request<Project[]>(`${BASE}/projects`),
  getProject: (id: number) => request<Project>(`${BASE}/projects/${id}`),
  createProject: (dto: CreateProject) =>
    request<Project>(`${BASE}/projects`, { method: 'POST', body: JSON.stringify(dto) }),
  updateProject: (id: number, dto: CreateProject) =>
    request<Project>(`${BASE}/projects/${id}`, { method: 'PUT', body: JSON.stringify(dto) }),
  deleteProject: (id: number) =>
    request<void>(`${BASE}/projects/${id}`, { method: 'DELETE' }),

  // Requirements
  listRequirements: (projectId: number) =>
    request<Requirement[]>(`${BASE}/projects/${projectId}/requirements`),
  createRequirement: (projectId: number, dto: CreateRequirement) =>
    request<Requirement>(`${BASE}/projects/${projectId}/requirements`, {
      method: 'POST',
      body: JSON.stringify(dto)
    }),
  updateRequirement: (projectId: number, id: number, dto: UpdateRequirement) =>
    request<Requirement>(`${BASE}/projects/${projectId}/requirements/${id}`, {
      method: 'PUT',
      body: JSON.stringify(dto)
    }),
  deleteRequirement: (projectId: number, id: number) =>
    request<void>(`${BASE}/projects/${projectId}/requirements/${id}`, { method: 'DELETE' })
};
