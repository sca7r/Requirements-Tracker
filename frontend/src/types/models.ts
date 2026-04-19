export type RequirementType = 'Functional' | 'NonFunctional';
export type Priority = 'Must' | 'Should' | 'Could' | 'Wont';
export type RequirementStatus = 'Draft' | 'Approved' | 'InProgress' | 'Done' | 'Rejected';

export interface Project {
  id: number;
  name: string;
  customerName: string;
  description?: string | null;
  requirementCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProject {
  name: string;
  customerName: string;
  description?: string | null;
}

export interface Requirement {
  id: number;
  projectId: number;
  title: string;
  description?: string | null;
  type: RequirementType;
  priority: Priority;
  status: RequirementStatus;
  estimatedHours?: number | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateRequirement {
  title: string;
  description?: string | null;
  type: RequirementType;
  priority: Priority;
  estimatedHours?: number | null;
}

export interface UpdateRequirement extends CreateRequirement {
  status: RequirementStatus;
}
