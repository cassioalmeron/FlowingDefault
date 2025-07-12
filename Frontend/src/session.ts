const TOKEN_KEY = 'token';

export const session = {
    getToken: () => {
        return localStorage.getItem(TOKEN_KEY);
    },
    isAuthenticated: () => {
        return localStorage.getItem(TOKEN_KEY) !== null;
    },
    logout: () => {
        localStorage.removeItem(TOKEN_KEY);
    },
    login: (token: string) => {
        localStorage.setItem(TOKEN_KEY, token);
    }
}