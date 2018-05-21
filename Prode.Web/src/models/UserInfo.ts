export interface IUserInfo {
    name?: string;
    mail?: string;
    hasPaid: boolean;
    isAdmin: boolean;
    id?: number;
}

export interface ProfileState {
    user?: IUserInfo;
}
