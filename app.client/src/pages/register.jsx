import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import CssBaseline from '@mui/material/CssBaseline';
import TextField from '@mui/material/TextField';
import React, { useState, useEffect } from 'react';
import { Link as RouterLink, useNavigate } from 'react-router-dom';
import Avatar from '@mui/material/Avatar';
import Grid from '@mui/material/Grid';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import Typography from '@mui/material/Typography';
import { useDispatch } from 'react-redux';
import CircularProgress from '@mui/material/CircularProgress';
import Alert from '@mui/material/Alert';

const Register = () => {
    const [formData, setFormData] = useState({
      userName: '',
      email: '',
      phoneNumber: '',
      password: '',
      confirmPassword: ''
    });
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(false);
    const [errors, setErrors] = useState({});
    const [message, setMessage] = useState(null);
    const dispatch = useDispatch();
    const navigate = useNavigate();

    useEffect(() => {
        if (message && message.type === 'success') {
            const timer = setTimeout(() => {
                navigate('/login');
            }, 3000);
            return () => clearTimeout(timer);
        }
    }, [message, navigate]);

    const validate = () => {
      let tempErrors = {};
      if (!formData.userName) tempErrors.userName = 'Имя пользователя обязательно';
      if (!formData.email) tempErrors.email = 'Электронная почта обязательна';
      if (!formData.password) tempErrors.password = 'Пароль обязателен';
      if (formData.password !== formData.confirmPassword) tempErrors.confirmPassword = 'Пароли не совпадают';
      setErrors(tempErrors);
      return Object.keys(tempErrors).length === 0;
    };

    const handleSubmit = async (event) => {
      event.preventDefault();
      if (!validate())
        return;
      //
      setLoading(true);
      setMessage(null);

      try {
        const response = await fetch('https://localhost:7086/account/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'                 
            },
            body: JSON.stringify(
              {
                UserName: formData.userName,
                Email: formData.email, 
                PhoneNumber: formData.phoneNumber,
                Password: formData.password,
                ConfirmPassword: formData.confirmPassword
              }
            )
        });

        if (response.ok) {
            const result = await response.json();
            setMessage({ type: 'success', text: 'Регистрация успешна!' });          
        } else {
          const errorResult = await response.json();

          //запишем ошибки
          let errorMessages = '';
          for(let error of errorResult)
            errorMessages += `${error}, `;
          errorMessages = errorMessages.slice(0, -2);
          //
          setMessage({ type: 'error', text: 'Ошибка регистрации: ' + errorMessages });
        }
      } catch (error) {
        console.error('Error:', error);
        setMessage({ type: 'error', text: 'Ошибка: ' + error.message });
      } finally {
        setLoading(false);
      }
    };

    const handleChange = (e) => {
      const { name, value } = e.target;
      setFormData((prevData) => ({
        ...prevData,
        [name]: value,
      }));
    };
    
    return (
      <Container component="main" maxWidth="xs">
          <CssBaseline />
          <Box
              sx={{
                  marginTop: 8,
                  display: 'flex',
                  flexDirection: 'column',
                  alignItems: 'center',
              }}
          >
              <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
                  <LockOutlinedIcon />
              </Avatar>
              <Typography component="h1" variant="h5">
                  Регистрация
              </Typography>
              {message && (
                  <Alert severity={message.type} sx={{ width: '100%', mb: 2 }}>
                      {message.text}
                  </Alert>
              )}
              <Box component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 3 }}>
                  <Grid container spacing={2}>
                      <Grid item xs={12}>
                          <TextField
                              autoComplete="given-name"
                              name="userName"
                              required
                              fullWidth
                              id="userName"
                              label="Имя пользователя"
                              autoFocus
                              onChange={handleChange}
                              error={!!errors.userName}
                              helperText={errors.userName}
                          />
                      </Grid>
                      <Grid item xs={12}>
                          <TextField
                              required
                              fullWidth
                              id="email"
                              label="Электронная почта"
                              name="email"
                              autoComplete="email"
                              onChange={handleChange}
                              error={!!errors.email}
                              helperText={errors.email}
                          />
                      </Grid>
                      <Grid item xs={12}>
                          <TextField
                              fullWidth
                              id="phoneNumber"
                              label="Номер телефона"
                              name="phoneNumber"
                              autoComplete="phoneNumber"
                              onChange={handleChange}
                          />
                      </Grid>
                      <Grid item xs={12}>
                          <TextField
                              required
                              fullWidth
                              name="password"
                              label="Пароль"
                              type="password"
                              id="password"
                              autoComplete="new-password"
                              onChange={handleChange}
                              error={!!errors.password}
                              helperText={errors.password}
                          />
                      </Grid>
                      <Grid item xs={12}>
                          <TextField
                              required
                              fullWidth
                              name="confirmPassword"
                              label="Повторите пароль"
                              type="password"
                              id="confirmPassword"
                              onChange={handleChange}
                              error={!!errors.confirmPassword}
                              helperText={errors.confirmPassword}
                          />
                      </Grid>
                  </Grid>
                  <Button
                      type="submit"
                      fullWidth
                      variant="contained"
                      sx={{ mt: 3, mb: 2 }}
                      disabled={loading}
                  >
                      {loading ? <CircularProgress size={24} /> : 'Зарегистрироваться'}
                  </Button>
                  <Grid container justifyContent="flex-end">
                      <Grid item>
                          <RouterLink to="/login" variant="body2">
                              Уже есть учетная запись? Войти
                          </RouterLink>
                      </Grid>
                  </Grid>
              </Box>
          </Box>
      </Container>
  );
};

export {Register};
