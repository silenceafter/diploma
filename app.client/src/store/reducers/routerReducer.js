import { UPDATE_ROUTE } from '../actions/routerActions';

const initialState = {
  currentRoute: '/',
};

const routerReducer = (state = initialState, action) => {
  switch (action.type) {
    case UPDATE_ROUTE:
      return {
        ...state,
        currentRoute: action.payload,
      };
    default:
      return state;
  }
};

export default routerReducer;