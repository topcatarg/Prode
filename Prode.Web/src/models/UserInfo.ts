export interface IUserInfo {
    name?: string;
    mail?: string;
    hasPaid: boolean;
    isAdmin: boolean;
    id?: number;
    gameGroupId: number;
    teamName: string;
    receiveMails: boolean;
    receiveAdminMails: boolean;
}

export interface ProfileState {
    user?: IUserInfo;
}
