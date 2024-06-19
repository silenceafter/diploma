import React, { useState } from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import CssBaseline from '@mui/material/CssBaseline';
import TextField from '@mui/material/TextField';
import { useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { Link } from "react-router-dom";
import Avatar from '@mui/material/Avatar';
import Grid from '@mui/material/Grid';
import LockResetIcon from '@mui/icons-material/LockReset';
import Typography from '@mui/material/Typography';
import { Link as RouterLink } from 'react-router-dom';
import { login } from '../store/actions/authActions';
import CircularProgress from '@mui/material/CircularProgress';

const Forgot = () => {
    const [formData, setFormData] = useState({ username: '', email: '', password: ''});
    const [loading, setLoading] = useState(false);
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const handleSubmit = async () => {
      setLoading(true);
        try {
          const response = await fetch('https://localhost:7086/account/forgot', {
              method: 'POST',
              headers: {
                  'Content-Type': 'application/json'                 
              },
              body: JSON.stringify({ Email: formData.email, Password: formData.password })
          });

          if (response.ok) {
              console.log('Login successful');
              const result = await response.json();

              //сохранить токен
              dispatch(login(result.userName, formData.email, result.token.result));           
              navigate('/');
          } else {
            console.error('Login failed');
          }
        } catch (error) {
          console.error('Error:', error);
        } finally {
          setLoading(false);
        }
    };   

    const handleChange = (event) => {
        setFormData({ ...formData, [event.target.name]: event.target.value });
      };
    //
    return (
          <Container component="main" maxWidth="xs">
            <CssBaseline />
            <Box
              sx={{
                marginTop: 8,
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center'
              }}
            >
              <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
                <LockResetIcon />
              </Avatar>
              <Typography component="h1" variant="h5">
                Восстановить пароль
              </Typography>
              <Box component="form" noValidate sx={{ mt: 1 }}>
                <TextField
                  margin="normal"
                  required
                  fullWidth
                  id="email"
                  label="Email Address"
                  name="email"
                  autoComplete="email"
                  autoFocus
                  value={formData.email}
                  onChange={handleChange}
                />                
                {loading 
                  ? (
                    <Box display="flex" justifyContent="center" sx={{ mt: 3, mb: 2 }}>
                      <CircularProgress />
                    </Box>
                    ) 
                  : (
                    <Button
                      type="button"
                      fullWidth
                      variant="contained"
                      sx={{ mt: 3, mb: 2 }}
                      onClick={() => handleSubmit()}
                    >
                      Восстановить
                    </Button>
                  )
                }
                <Grid container>
                  <Grid item xs>
                    <Link component={RouterLink} to="/forgot" variant="body2">
                      Вспомнили пароль?
                    </Link>
                  </Grid>                  
                </Grid>
              </Box>
            </Box>    
            </Container>
      );
};

export {Forgot};