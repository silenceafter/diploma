import React, { useState, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { logout } from '../store/actions/authActions';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import CssBaseline from '@mui/material/CssBaseline';
import Typography from '@mui/material/Typography';
import CircularProgress from '@mui/material/CircularProgress';
import Backdrop from '@mui/material/Backdrop';
import { makeStyles } from '@mui/styles';

const useStyles = makeStyles((theme) => ({
    backdrop: {
        zIndex: theme.zIndex.drawer + 1,
        color: '#fff',
        backdropFilter: 'blur(5px)',//блюра
        backgroundColor: 'rgba(255, 255, 255, 0.5)',//светлый цвет с прозрачностью
    },
}));

const Logout = () => {
    const { token } = useSelector((state) => state.auth);
    const [loading, setLoading] = useState(true);
    const classes = useStyles();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    
    useEffect(() => {    
        const handleSubmit = async () => {
            try {
                const response = await fetch('https://localhost:7086/account/logout', {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${token}`                 
                    }
                });

                if (response.ok) {
                    console.log('Logout successful');                    
                } else {
                console.error('Logout failed');
                }
            } catch (error) {
                console.error('Error:', error);
            } finally {
                //удалить токен
                dispatch(logout());
                navigate('/');
                setLoading(false);
            }
        };
        handleSubmit();
    }, []);
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
                {loading ? (
                    <Backdrop className={classes.backdrop} open={loading}>
                        <CircularProgress />
                    </Backdrop>
                ) : (
                    <Typography component="h1" variant="h5">
                        Logout
                    </Typography>
                )}
            </Box>
        </Container>
    );
};

export {Logout};