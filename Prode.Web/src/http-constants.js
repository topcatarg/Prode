import axios from 'axios';
let baseURL; 
if (!process.env.NODE_ENV || process.env.NODE_ENV === 'development' ) {
    baseURL = 'https://localhost:5000/' 
} 
else { 
    baseURL = 'https://traducir.win/app/' 
} 
export const HTTP = axios.create ( { baseURL : baseURL }); 