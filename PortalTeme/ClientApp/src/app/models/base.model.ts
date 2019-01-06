export interface BaseModel {
    id: string;
}

export interface EditModel {
    id?: string;    
}

export interface NamedModel extends BaseModel {
    name: string;
}