const USER_KEY = 'user';

interface User {
    name: string;
    isAdmin: boolean;
    token: string;
    expires: Date;
}

export const session = {
    getToken: () => {
        const user = session.getUser();
        return user?.token ?? null;
    },
    getUser: () => {
        const user = localStorage.getItem(USER_KEY);
        if (user) {
            return JSON.parse(user) as User;
        }
        return null;
    },
    isTokenExpired: () => {
        const user = session.getUser();
        if (!user || !user.expires) {
            return true;
        }
        
        // Convert expires string back to Date if it was serialized
        const expirationDate = typeof user.expires === 'string' 
            ? new Date(user.expires) 
            : user.expires;
            
        return new Date() >= expirationDate;
    },
    isAuthenticated: () => {
        const user = localStorage.getItem(USER_KEY);
        if (!user) {
            return false;
        }
        
        // Check if token has expired
        if (session.isTokenExpired()) {
            session.logout();
            return false;
        }
        
        return true;
    },
    logout: () => {
        localStorage.removeItem(USER_KEY);
    },
    login: (user: User) => {
        localStorage.setItem(USER_KEY, JSON.stringify(user));
    },
    // Function to start periodic token expiration checking
    startExpirationCheck: (checkIntervalMs: number = 60000) => {
        const checkExpiration = () => {
            if (session.isTokenExpired()) {
                session.logout();
                window.location.href = '/login';
            }
        };
        
        // Check immediately
        checkExpiration();
        
        // Set up periodic checking
        return setInterval(checkExpiration, checkIntervalMs);
    }
}