import Box from '@mui/material/Box';
import Link from '@mui/material/Link';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import { useSelector, useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import React, { useEffect, useState } from 'react';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import Sidebar from '../components/sidebar';
import MainFeaturedPost from '../components/mainFeaturedPost';
import Divider from '@mui/material/Divider';
import image from '../images/account.jpg';
import {
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
} from '@mui/material';
import { fetchDataGet } from '../store/thunk/thunks';
import { updateRoute } from '../store/actions/routerActions';

function formatDateTime(dateTimeString) {
    const date = new Date(dateTimeString);
    const utcTime = date.getTime();
    const moscowOffset = 3 * 60 * 60 * 1000;
    const moscowDate = new Date(utcTime + moscowOffset);
    const options = {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
    };

    // Форматируем дату с учетом указанных опций
    const formattedDateTime = moscowDate.toLocaleString('ru-RU', options);
    return formattedDateTime;
}

const Account = () => {
    const { email, token, role } = useSelector((state) => state.auth);
    const { social } = useSelector((state) => state.social);    
    const currentRoute = useSelector(state => state.router.currentRoute);
    //
    const dispatch = useDispatch();
    const navigate = useNavigate();

    //запросы
    const userDataRequest = useSelector((state) => state.getRequest.userDataRequest);
    const userTransactionsRequest = useSelector((state) => state.getRequest.userTransactionsRequest);
    const userAcceptanceRequest = useSelector((state) => state.getRequest.userAcceptanceRequest);
    const userReceivingDiscountRequest = useSelector((state) => state.getRequest.userReceivingDiscountRequest);
    const userReceivingProductRequest = useSelector((state) => state.getRequest.userReceivingProductRequest);

    //роли
    const roles = [ 
        { name: 'User', nameRu: 'Пользователь' },
        { name: 'Admin', nameRu: 'Администратор' },
        { name: 'Worker', nameRu: 'Сотрудник' }
    ];

    const handleRouteChange = (newRoute) => {
        dispatch(updateRoute(newRoute));
    };

    const getRoleNameRu = (role) => {
        const roleObject = Array.from(roles).find(r => r.name === role);
        return roleObject ? roleObject.nameRu : 'Роль не найдена';
    };    

    //раздел
    const mainFeaturedPost = {
        title: 'Учетная запись',
        description: "",
        image: image,
        imageText: '',
        linkText: '',
    };

    //информация о странице
    const sidebar = {
        title: '',
        description:
            'Данные Вашей учетной записи, количество бонусных баллов, информация о списании/начислении, последних совершенных действиях и т.д.',    
        social: social
    };    

    //useEffect
    useEffect(() => {
        //доступ запрещен
        if (!email)
            return;//navigate('/access-denied');

        //запрос данных пользователя
        dispatch(fetchDataGet(token, 'https://localhost:7158/user/get-current-user', 'userDataRequest'));
        dispatch(fetchDataGet(token, 'https://localhost:7158/user/get-user-transactions', 'userTransactionsRequest'));
        dispatch(fetchDataGet(token, 'https://localhost:7158/user/get-user-acceptance', 'userAcceptanceRequest'));
        dispatch(fetchDataGet(token, 'https://localhost:7158/user/get-user-receiving-discount', 'userReceivingDiscountRequest'));
        dispatch(fetchDataGet(token, 'https://localhost:7158/user/get-user-receiving-product', 'userReceivingProductRequest'));        
    }, [dispatch]);

    //состояния запросов
    const isLoading = userDataRequest?.loading || userTransactionsRequest?.loading || userAcceptanceRequest?.loading || 
        userReceivingDiscountRequest?.loading || userReceivingProductRequest?.loading;
    const isError = userDataRequest?.error || userTransactionsRequest?.error || userAcceptanceRequest?.error || 
        userReceivingDiscountRequest?.error || userReceivingProductRequest?.error;

    //рендер
    //доступ запрещен
    if (!email)
        navigate('/access-denied');

    if (isLoading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
                <CircularProgress />
            </Box>
        );
    }
    //
    if (isError) {
        if (userDataRequest.error === 401 || userTransactionsRequest.error === 401 || userAcceptanceRequest.error || 
            userReceivingDiscountRequest.error || userReceivingProductRequest.error) {
            navigate('/login');
        }
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
                { role != null && role == 'Admin' && (
                    <>
                    <Divider />
                    <div>
                        <Box sx={{ textAlign: 'justify', mt: 2, mb: 5 }}>
                            <Typography variant="h6" gutterBottom sx={{ mt: 5 }}>
                                Панель администратора
                            </Typography>
                            <Typography><Link href="#">Пользователи</Link></Typography>
                            <Typography><Link href="#">Магазины</Link></Typography>
                            <Typography><Link href="#">Пункты приема отходов</Link></Typography>
                            <Typography><Link href="#">Отходы</Link></Typography>
                            <Typography><Link href="#">Купоны</Link></Typography>
                            <Typography><Link href="#">Товары</Link></Typography>
                        </Box>
                    </div>
                    </>
                )}

                { role != null && role == 'Worker' && (
                    <>
                    <Divider />
                    <div>
                        <Box sx={{ textAlign: 'justify', mt: 2, mb: 5 }}>
                            <Typography variant="h6" gutterBottom sx={{ mt: 5 }}>
                                Панель сотрудника
                            </Typography>
                            <Typography><Link href="#">Пользователи</Link></Typography>
                            <Typography><Link href="#">Магазины</Link></Typography>
                            <Typography><Link href="#">Пункты приема отходов</Link></Typography>
                            <Typography><Link href="#">Отходы</Link></Typography>
                            <Typography><Link href="#">Купоны</Link></Typography>
                            <Typography><Link href="#">Товары</Link></Typography>
                        </Box>
                    </div>
                    </>
                )}

                <Divider />
                <div>
                    <Box sx={{ textAlign: 'justify', mt: 2, mb: 5 }}>
                        <Typography variant="h6" gutterBottom sx={{ mt: 5 }}>
                            Учетные данные
                        </Typography>
                        <TableContainer component={Paper}>
                            <Table aria-label="user table">
                                <TableHead>
                                <TableRow>
                                    <TableCell>Имя пользователя</TableCell>
                                    <TableCell>Email</TableCell>
                                    <TableCell>Роль</TableCell>
                                    <TableCell>Номер телефона</TableCell>
                                    <TableCell>Кол-во баллов</TableCell>
                                </TableRow>
                                </TableHead>
                                <TableBody>
                                <TableRow>
                                { userDataRequest?.data
                                    ? (
                                        <>
                                            <TableCell>{userDataRequest?.data.userName}</TableCell>
                                            <TableCell>{userDataRequest?.data.email}</TableCell>
                                            <TableCell>{getRoleNameRu(userDataRequest?.data.role)}</TableCell>
                                            <TableCell>{userDataRequest?.data.phoneNumber != null ? userDataRequest?.data.phoneNumber : 'не указан'}</TableCell>
                                            <TableCell>{userDataRequest?.data.bonus}</TableCell>
                                        </>
                                    ) 
                                    : (
                                        <>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                        </>
                                    )
                                }  
                                </TableRow>
                                </TableBody>
                            </Table>
                        </TableContainer>                        
                    </Box>
                    <Box sx={{ textAlign: 'justify', mt: 2, mb: 2 }}>
                        <Typography variant="h6" gutterBottom sx={{ mt: 5 }}>
                            Совершенные операции
                        </Typography>
                        <TableContainer component={Paper}>
                            <Table aria-label="user table">
                                <TableHead>
                                <TableRow>
                                    <TableCell>#</TableCell>
                                    <TableCell>Операция</TableCell>
                                    <TableCell>Дата</TableCell>
                                    <TableCell>Кол-во бонусов было</TableCell>
                                    <TableCell>Кол-во бонусов стало</TableCell>
                                </TableRow>
                                </TableHead>
                                <TableBody>                          
                                { userTransactionsRequest?.data
                                    ? (                                                                                
                                        userTransactionsRequest.data.map((transaction, index) => (
                                            <>
                                                <TableRow key={transaction.id}>
                                                    <TableCell>{index + 1}</TableCell>
                                                    <TableCell>{transaction.transactionName}</TableCell>
                                                    <TableCell>{formatDateTime(transaction.date)}</TableCell>
                                                    <TableCell>{transaction.bonusesStart}</TableCell>
                                                    <TableCell>{transaction.bonusesEnd}</TableCell>
                                                </TableRow>
                                            </>
                                        ))                                                  
                                    ) 
                                    : (
                                        <>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                        </>
                                    )
                                }  
                                </TableBody>
                            </Table>
                        </TableContainer>
                    </Box>
                    <Box sx={{ textAlign: 'justify', mt: 2, mb: 2 }}>
                        <Typography variant="h6" gutterBottom sx={{ mt: 5 }}>
                            Операции приема отходов
                        </Typography>
                        <TableContainer component={Paper}>
                            <Table aria-label="user table">
                                <TableHead>
                                <TableRow>
                                    <TableCell>#</TableCell>
                                    <TableCell>Отход</TableCell>
                                    <TableCell>Кол-во</TableCell>
                                    <TableCell>Пункт приема</TableCell>
                                    <TableCell>Адрес пункта приема</TableCell>
                                    <TableCell>Дата</TableCell>
                                </TableRow>
                                </TableHead>
                                <TableBody>                          
                                { userAcceptanceRequest?.data
                                    ? (                                                                                
                                        userAcceptanceRequest.data.map((transaction, index) => (
                                            <>
                                                <TableRow key={transaction.id}>
                                                    <TableCell>{index + 1}</TableCell>
                                                    <TableCell>{transaction.hazardousWasteName}</TableCell>
                                                    <TableCell>{transaction.quantity}</TableCell>
                                                    <TableCell>{transaction.pointName}</TableCell>
                                                    <TableCell>{transaction.pointAddress}</TableCell>
                                                    <TableCell>{formatDateTime(transaction.date)}</TableCell>
                                                </TableRow>
                                            </>
                                        ))                                                  
                                    ) 
                                    : (
                                        <>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                        </>
                                    )
                                }  
                                </TableBody>
                            </Table>
                        </TableContainer>
                    </Box>
                    <Box sx={{ textAlign: 'justify', mt: 2, mb: 2 }}>
                        <Typography variant="h6" gutterBottom sx={{ mt: 5 }}>
                            Операции обмена бонусов на купоны
                        </Typography>
                        <TableContainer component={Paper}>
                            <Table aria-label="user table">
                                <TableHead>
                                <TableRow>
                                    <TableCell>#</TableCell>
                                    <TableCell>Магазин</TableCell>
                                    <TableCell>Условия применения</TableCell>
                                    <TableCell>Начало действия</TableCell>
                                    <TableCell>Окончание действия</TableCell>
                                    <TableCell>Кол-во бонусов</TableCell>
                                    <TableCell>Дата</TableCell>
                                </TableRow>
                                </TableHead>
                                <TableBody>                          
                                { userReceivingDiscountRequest?.data
                                    ? (                                                                                
                                        userReceivingDiscountRequest.data.map((transaction, index) => (
                                            <>
                                                <TableRow key={transaction.id}>
                                                    <TableCell>{index + 1}</TableCell>
                                                    <TableCell>{transaction.partnerName}</TableCell>
                                                    <TableCell>{transaction.discountTerms}</TableCell>
                                                    <TableCell>{formatDateTime(transaction.discountDateStart)}</TableCell>
                                                    <TableCell>{formatDateTime(transaction.discountDateEnd)}</TableCell>
                                                    <TableCell>{transaction.discountBonuses}</TableCell>
                                                    <TableCell>{formatDateTime(transaction.date)}</TableCell>
                                                </TableRow>
                                            </>
                                        ))                                                  
                                    ) 
                                    : (
                                        <>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                        </>
                                    )
                                }  
                                </TableBody>
                            </Table>
                        </TableContainer>
                    </Box>
                    <Box sx={{ textAlign: 'justify', mt: 2, mb: 2 }}>
                        <Typography variant="h6" gutterBottom sx={{ mt: 5 }}>
                            Операции обмена бонусов на товары
                        </Typography>
                        <TableContainer component={Paper}>
                            <Table aria-label="user table">
                                <TableHead>
                                <TableRow>
                                    <TableCell>#</TableCell>
                                    <TableCell>Товар</TableCell>
                                    <TableCell>Бонусы</TableCell>
                                    <TableCell>Количество</TableCell>                                    
                                    <TableCell>Стоимость</TableCell>
                                    <TableCell>Дата</TableCell>
                                </TableRow>
                                </TableHead>
                                <TableBody>                          
                                { userReceivingProductRequest?.data
                                    ? (                                                                                
                                        userReceivingProductRequest.data.map((transaction, index) => (
                                            <>
                                                <TableRow key={transaction.id}>
                                                    <TableCell>{index + 1}</TableCell>
                                                    <TableCell>{transaction.productName}</TableCell>
                                                    <TableCell>{transaction.productBonus}</TableCell>
                                                    <TableCell>{transaction.productQuantity}</TableCell>                                                    
                                                    <TableCell>{transaction.cost}</TableCell>
                                                    <TableCell>{formatDateTime(transaction.date)}</TableCell>
                                                </TableRow>
                                            </>
                                        ))                                                  
                                    ) 
                                    : (
                                        <>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>                    
                                            <TableCell></TableCell>
                                            <TableCell></TableCell>
                                        </>
                                    )
                                }  
                                </TableBody>
                            </Table>
                        </TableContainer>
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

export {Account};