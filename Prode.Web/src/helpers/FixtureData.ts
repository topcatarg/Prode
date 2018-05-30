export default interface IFixtureData {
     id: number;
     date: Date;
     standardDate: string;
     team1: number;
     team2: number;
     team1Name: string;
     team2Name: string;
     team1Flag: string;
     team2Flag: string;
     team1Goals?: number;
     team2Goals?: number;
     wwGroup: string;
     team1Forecast?: number;
     team2Forecast?: number;
     points: number;
     canUpdate: boolean;
     closed: boolean;
}
