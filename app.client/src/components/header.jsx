import * as React from 'react';
import PropTypes from 'prop-types';
import Toolbar from '@mui/material/Toolbar';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import SearchIcon from '@mui/icons-material/Search';
import Typography from '@mui/material/Typography';
import Link from '@mui/material/Link';
import { Link as RouterLink } from 'react-router-dom';
import { useState, useEffect } from 'react';
//import { Link } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import { logout } from '../store/actions/authActions';
import { styled } from '@mui/system';
import { Box } from '@mui/material';
import logo from '../images/logo.png';
import { updateRoute } from '../store/actions/routerActions';

function Header(props) {
  const { /*sections,*/ title } = props;
  const { username, email, token } = useSelector((state) => state.auth);
  const dispatch = useDispatch();

  React.useEffect(() => {
    dispatch(updateRoute(location.pathname));
  }, [location.pathname, dispatch]);

  //список страниц для header
  let sections = [];
  if (email) {
    //для авторизованных пользователей
    sections = [
      { title: 'Главная', url: '/' },
      { title: 'О нас', url: '/about' },
      { title: 'Принять отходы', url: '/acceptance' },
      { title: 'Обменять бонусы', url: '/bonus-exchange' },      
      { title: 'Магазины', url: '/partners' },            
      { title: 'Наши товары', url: '/products' }, 
      { title: 'Пункты приёма отходов', url: '/points' },          
    ];
  } else {
    //для гостей
    sections = [
      { title: 'Главная', url: '/' },
      { title: 'О нас', url: '/about' },
      { title: 'Магазины', url: '/partners' },
      { title: 'Наши товары', url: '/products' },
      { title: 'Пункты приёма отходов', url: '/points' },
    ];
  }

  //кастомный RouterLink
  const StyledRouterLink = styled(RouterLink)(({ theme, active }) => ({
    color: active ? theme.palette.primary.main : 'inherit',
    textDecoration: 'none',
    padding: '8px',
    flexShrink: 0,
    fontWeight: active ? 'bold' : 'normal',
  }));
  //
  return (
    <React.Fragment>
      <Toolbar sx={{ borderBottom: 1, borderColor: 'divider' }}>
      <Box display="flex" justifyContent="center" mb={2}>
        <img src={logo} alt="Logo" style={{ height: '70px' }} /> {/* Используйте логотип */}
      </Box>
        <Typography
          component="h2"
          variant="h5"
          color="inherit"
          align="center"
          noWrap
          sx={{ flex: 1 }}
        >
          {title}
        </Typography>       
       { email 
          ? (
            <>
              <Box
                display="flex"
                flexDirection="row"
                gap={2}
                alignItems="center"
              >
                {/*<IconButton>
                  <SearchIcon />
                </IconButton>*/}
                <Typography component={RouterLink} to="/account" sx={{ textDecoration: 'none' }}>{username}</Typography>
                <Button variant="outlined" size="small" component={RouterLink} to="/logout">
                  Выйти
                </Button>
              </Box>              
            </>
            ) 
          : (
            <>
              <Box
                display="flex"
                flexDirection="row"
                gap={2}
                alignItems="center"
              >
                {/*<IconButton>
                  <SearchIcon />
                </IconButton>*/}                
                <Button variant="outlined" size="small" component={RouterLink} to="/login">
                  Войти
                </Button>
              </Box>              
            </>
            )
        }                           
      </Toolbar>
      <Toolbar
        component="nav"
        variant="dense"
        sx={{ justifyContent: 'space-between', overflowX: 'auto' }}
      >
        {sections.map((section) => (
          <StyledRouterLink
            key={section.title}
            to={section.url}
            sx={{ p: 1, flexShrink: 0, }}
          >
            {section.title}
          </StyledRouterLink>
        ))}
      </Toolbar>
    </React.Fragment>
  );
}

Header.propTypes = {
  /*sections: PropTypes.arrayOf(
    PropTypes.shape({
      title: PropTypes.string.isRequired,
      url: PropTypes.string.isRequired,
    }),
  ).isRequired,*/
  title: PropTypes.string.isRequired,
};
//<Link to="/">Главная страница</Link>
export default Header;