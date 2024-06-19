import Typography from '@mui/material/Typography';
import { useSelector, useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import React, { useEffect, useState } from 'react';
import Sidebar from '../components/sidebar';
import MainFeaturedPost from '../components/mainFeaturedPost';
import image from '../images/bonusExchange.jpg';
import { Divider } from '@mui/material';
import { Grid, TextField, Box } from '@mui/material';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import { logout } from '../store/actions/authActions';
import { Card, CardContent, Radio } from '@mui/material';
import { Alert } from '@mui/material';

function formatDate(dateString) {
    //форматирование даты в дд.мм.гггг
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0'); // Месяцы от 0 до 11
    const year = date.getFullYear();
    return `${day}.${month}.${year}`;
}

const BonusExchange = () => {
    const { email, token } = useSelector((state) => state.auth);
    const [userData, setUserData] = useState(null);
    const [userBalance, setUserBalance] = useState(null);
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(true);
    const [selectedDiscountId, setSelectedDiscountId] = useState(null);
    const [selectedDiscountBonus, setSelectedDiscountBonus] = useState(null);
    const [submitLoading, setSubmitLoading] = useState(false);
    const [submitResult, setSubmitResult] = useState(null);
    const { social } = useSelector((state) => state.social);
    //
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const PlaceholderCard = ({ couponNumber }) => (
        <Card style={{ margin: '10px', padding: '20px', backgroundColor: '#f5f5f5' }}>
          <CardContent>
            <Typography variant="h6" component="div">Купон {couponNumber}</Typography>
            <Typography variant="body2" component="div" sx={{ textAlign: 'left' }}>Ошибка загрузки данных</Typography>
          </CardContent>
        </Card>
    );

    const handleSelect = (id, bonus) => {
        setSelectedDiscountId(id);//храним выбранный id карты
        setSelectedDiscountBonus(bonus);//храним количество бонусов, которое стоит купон
    };
    const DiscountCard = ({ discount, isSelected, onSelect }) => (
        <Card
          key={discount.id}
          onClick={() => onSelect(discount.id, discount.bonus)}
          style={{
            margin: '10px',
            cursor: 'pointer',
            border: isSelected ? '2px solid #3f51b5' : '1px solid #ccc'
          }}
        >
          <CardContent>        
            <Typography variant="body2" component="div"><strong>Описание:</strong> {discount.conditions}</Typography>
            <Box mt={0.5}>
                <Typography variant="body2" component="div">
                    <strong>Магазин:</strong> {discount.storeName}
                </Typography>
            </Box>
            <Box mt={0.5}>
                <Typography variant="body2" component="div" align="left">
                    <strong>Действует:</strong> {formatDate(discount.dateStart)} - {formatDate(discount.dateEnd)}
                </Typography>
            </Box>
            <Radio
              checked={isSelected}
              value={discount.id}
              readOnly
            />
          </CardContent>
        </Card>
    );
    const DiscountCardList = ({ discounts, error }) => {
        return (            
          <Grid container spacing={2} sx={{ maxHeight: 600, overflowY: 'auto', p: 2 }}>
            {error ? (
              // Если есть ошибка, отображаем заглушки
              <>
                <Grid item xs={12} sm={6} md={4}><PlaceholderCard couponNumber={1} /></Grid>
                <Grid item xs={12} sm={6} md={4}><PlaceholderCard couponNumber={2} /></Grid>
                <Grid item xs={12} sm={6} md={4}><PlaceholderCard couponNumber={3} /></Grid>
              </>
            ) : (
              // В противном случае отображаем реальные данные
              discounts.map((discount) => (
                <Grid item xs={12} sm={6} md={4} key={discount.id}>
                  <DiscountCard
                    discount={discount}
                    isSelected={selectedDiscountId === discount.id}
                    onSelect={handleSelect}
                  />
                </Grid>
              ))
            )}
          </Grid>
        );
    };

    //раздел
    const mainFeaturedPost = {
        title: 'Обменять бонусы',
        description: "",
        image: image,
        imageText: 'main image description',
        linkText: '',
    };

    //информация о странице
    const sidebar = {
        title: '',
        description: 'Спасибо, что участвуете в нашем проекте и поддерживаете экологически ответственное поведение! Если у вас возникли вопросы или нужна дополнительная информация, свяжитесь с нами',        
        social: social,
    };

    //собрать данные из элементов и отправить запрос на сервер
    const handleSubmit = async (event) => {
        event.preventDefault();
        setSubmitLoading(true);
        const dataToSend = {
            email: email,
            discountId: selectedDiscountId
        };
    
        try {
          const response = await fetch('https://localhost:7158/receivingdiscount/register-discount-reserve', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(dataToSend),
          });
    
          if (!response.ok) {
            if (response.status === 401) {
                dispatch(logout());
                navigate('/login');
            }
          }
    
          const result = await response.json();
          setSubmitResult({ success: true, message: 'Данные приняты!' });
        } catch (error) {
          console.error('Error:', error);
          setSubmitResult({ success: false, message: 'Ошибка. Данные не приняты.' });
        } finally {
            setSubmitLoading(false);
            setTimeout(() => setSubmitResult(null), 3000);

            //обновляем баланс после успешного запроса
            try {
                const response2 = await fetch('https://localhost:7158/user/get-account-balance', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json',
                    }
                });
    
                if (response2.ok) {
                    const balanceResponse = await response2.json();
                    setUserBalance(balanceResponse);
                } else {
                    if (response2.status === 401) {
                        dispatch(logout());
                        navigate('/login');
                    }
                }
            } catch (error) {
                setError(error);
            }
        }
      };  

    //useEffect
    useEffect(() => {
        //доступ запрещен
        if (!email)
            return;//navigate('/access-denied');
        
        //1 получить список доступных купонов
        const userRequest = async () => {
            try {
                const response = await fetch('https://localhost:7158/receivingdiscount/get-discounts', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json',
                    }
                });
                //
                if (response.ok) {
                    const userResponse = await response.json();
                    let items = [];
                    for(const item of userResponse) {
                        items.push(
                            {
                                id: item.id,
                                storeName: item.partner.name,
                                conditions: item.terms,
                                bonus: item.bonuses,
                                dateStart: item.dateStart,
                                dateEnd: item.dateEnd
                            }
                        );
                    }
                    setUserData(items);

                    //2 запрос баланса
                    try {
                        const response2 = await fetch('https://localhost:7158/user/get-account-balance', {
                            method: 'GET',
                            headers: {
                                'Authorization': `Bearer ${token}`,
                                'Content-Type': 'application/json',
                            }
                        });
                        //
                        if (response2.ok) {
                            const balanceResponse = await response2.json();
                            setUserBalance(balanceResponse);
                        } else {
                            if (response.status === 401) {
                                //токен не действует
                                dispatch(logout());//удалить токен
                                navigate('/login');
                            }
                        }
                    } catch(error) {
                        setError(error);
                    }
                } else {
                    if (response.status === 401) {
                        //токен не действует
                        dispatch(logout());//удалить токен
                        navigate('/login');
                    }                
                }
            } catch (error) {
                setError(error);
            } finally {
                setLoading(false);
            }
        };
        userRequest();
    }, []);

    //рендер
    //доступ запрещен
    if (!email)
        navigate('/access-denied');
    if (loading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
                <CircularProgress />
            </Box>
        );
    }
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
                                Обмен бонусов
                            </Typography>
                            <Typography variant="body1" paragraph>
                                Выберите скидочный купон из списка. Подтвердите списание бонусных баллов со счета.
                            </Typography>
                            <DiscountCardList discounts={userData} error={!!error} onSelect={handleSelect} />
                            <Box sx={{ mt: 2, mb: 2 }}>
                                <TextField
                                    label='Кол-во бонусов'
                                    value={userBalance?.bonus}
                                    variant="outlined"
                                    fullWidth
                                    InputProps={{
                                        readOnly: true,
                                    }}
                                    margin="normal"
                                />
                                {selectedDiscountBonus && (
                                    <Typography variant="body1" paragraph>
                                        Будет списано { selectedDiscountBonus } бонусов. Баллов на счете 
                                        <Box component="span" sx={{ color: userBalance?.bonus >= selectedDiscountBonus ? 'inherit' : 'red' }}>
                                            { userBalance?.bonus >= selectedDiscountBonus ? ' достаточно' : ' не достаточно' }.
                                        </Box>
                                    </Typography>
                                )}                                
                                <Button                            
                                    variant="contained"
                                    color="secondary"
                                    type="submit"
                                    onClick={handleSubmit}
                                    disabled={submitLoading}
                                    startIcon={submitLoading ? <CircularProgress size={24} /> : null}
                                >                                    
                                    Подтвердить
                                </Button>
                            </Box>
                            {submitResult && (
                                <Box sx={{ mt: 2 }}>
                                    <Alert severity={submitResult.success ? 'success' : 'error'}>
                                        {submitResult.message}
                                    </Alert>
                                </Box>
                            )}
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

export {BonusExchange};