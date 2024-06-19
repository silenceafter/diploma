const initialState = {
    username: localStorage.getItem('auth') ? JSON.parse(localStorage.getItem('auth')).username : null,
    email: localStorage.getItem('auth') ? JSON.parse(localStorage.getItem('auth')).email : null,
    token: localStorage.getItem('auth') ? JSON.parse(localStorage.getItem('auth')).token : null,
    role: localStorage.getItem('auth') ? JSON.parse(localStorage.getItem('auth')).role : null,
};

const authReducer = (state = initialState, action) => {
    switch (action.type) {
      case 'LOGIN':        
        const loginData = {
          username: action.payload.username,
          email: action.payload.email,
          token: action.payload.token,
          role: action.payload.role,
        };
        localStorage.setItem('auth', JSON.stringify(loginData));
        return {
          ...state,
          ...loginData,
        };

      case 'LOGOUT':
        localStorage.removeItem('auth');
        return {
          ...state,
          ...{
            username: null,
            email: null,
            token: null,
            role: null,
          },
        };

      default:
        return state;
    }
  };
  
  export default authReducer;