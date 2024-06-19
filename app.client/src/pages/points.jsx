import React, { useEffect, useState } from 'react';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import { useSelector, useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import MainFeaturedPost from '../components/mainFeaturedPost';
import image from '../images/points.jpg';
import { Divider } from '@mui/material';
import { Grid, TextField, Box  } from '@mui/material';
import Sidebar from '../components/sidebar';
import {
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
  } from '@mui/material';


const Points = () => {
    const [userData, setUserData] = useState([]);
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(true);
    const { email, token } = useSelector((state) => state.auth);
    const { social } = useSelector((state) => state.social);
    const dispatch = useDispatch();
    const navigate = useNavigate();  
    
    //раздел
    const mainFeaturedPost = {
        title: 'Пункты приёма отходов',
        description: "",
        image: image,
        imageText: 'main image description',
        linkText: '',
    };

    //информация о странице
    const sidebar = {
        title: '',
        description: 'Спасибо за ваше участие в нашем проекте и стремление к экологической ответственности! Если у вас возникли вопросы или вам нужна дополнительная информация, свяжитесь с нами',
        social: social,
    };

    //
    useEffect(() => {
        const userRequest = async () => {
            try {
                const response = await fetch('https://localhost:7158/point/get-points');
                if (response.ok) {
                    const userResponse = await response.json();
                    setUserData(userResponse);
                }
            } catch (error) {
                setError(error);
            } finally {
                setLoading(false);
            }
        };
        userRequest();
    }, []);

    //добавляем строки в таблицу
    const tableRows = [];
    for (let i = 0; i < userData.length; i++) {
        tableRows.push(
        <TableRow>
            <TableCell>{i + 1}</TableCell>
            <TableCell>{userData[i]?.name}</TableCell>
            <TableCell>{userData[i]?.address}</TableCell>
        </TableRow>
        );
    }
    //
    return (
        <> 
            <MainFeaturedPost post={mainFeaturedPost} />   
            <Grid container spacing={5} sx={{ mt: 3 }}>                           
                <Grid
                    item
                    xs={12}
                    md={8}
                    sx={{
                        '& .markdown': {
                        py: 3,
                        },
                    }}
                >
                    <Divider />
                    <div>                                                 
                        <Box sx={{ textAlign: 'justify', mt: 2, mb: 2 }}>
                            <Typography variant="h4" component="h1" gutterBottom>
                                Список пунктов приёма отходов
                            </Typography>
                            <Typography variant="body1" paragraph></Typography>            
                            <div>
                                <TableContainer component={Paper}>
                                    <Table aria-label="user table">
                                    <TableHead>
                                        <TableRow>
                                        <TableCell>#</TableCell>
                                        <TableCell>Название</TableCell>
                                        <TableCell>Адрес</TableCell>
                                        </TableRow>
                                    </TableHead>
                                    <TableBody>{tableRows}</TableBody>
                                    </Table>
                                </TableContainer>
                            </div>
                        </Box>                                    
                    </div>
                </Grid> 
                <Sidebar
                    title={sidebar.title}
                    description={sidebar.description}
                    archives={sidebar.archives}
                    social={sidebar.social}
                /> 
            </Grid>           
        </>
    );
};

export {Points};