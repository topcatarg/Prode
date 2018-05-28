export default class IFixtureTableData {
    public id: number = 0;
    public wwGroup: string = '';
    public canUpdate: boolean = true;
}

export interface IFixtureTableData2 {
    id: number;
    wwGroup: string;
    canUpdate: boolean;
    goals1forecast: number;
    goals2forecast: number;
}
