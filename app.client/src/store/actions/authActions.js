export const login = (username, email, token, role) => ({
    type: 'LOGIN',
    payload: { username, email, token, role },
});

export const logout = () => ({
    type: 'LOGOUT',
});