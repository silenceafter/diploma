import Typography from '@mui/material/Typography';
import { useSelector, useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import React, { useEffect, useState } from 'react';
import Sidebar from '../components/sidebar';
import MainFeaturedPost from '../components/mainFeaturedPost';
import image from '../images/products.jpg';
import { Divider } from '@mui/material';
import { Grid, TextField, Box } from '@mui/material';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import { logout } from '../store/actions/authActions';
import { Card, CardContent, Radio } from '@mui/material';
import { Alert } from '@mui/material';
import { Paper } from '@mui/material';
import { CardMedia } from '@mui/material';
import product_1 from '../images/shopper_1.jpg';
import product_2 from '../images/cap_2.jpg';
import product_3 from '../images/t-shirt_3.jpg';
import product_4 from '../images/hoodie_4.jpg';

const productImages = {
    1: product_1,
    2: product_2,
    3: product_3,
    4: product_4
  };

function getCorrectForm(quantity, forms) {
    const lastDigit = quantity % 10;
    const lastTwoDigits = quantity % 100;

    if (lastTwoDigits >= 11 && lastTwoDigits <= 19)
        return forms[2];
    if (lastDigit === 1)
        return forms[0];
    if (lastDigit >= 2 && lastDigit <= 4)
        return forms[1];
    return forms[2];
}

const ProductCard = ({ product, onQuantityChange, role }) => {
    const imageUrl = productImages[product.id];
    return (
        <Card style={{ margin: '20px', width: '300px' }}>
          <CardMedia
            component="img"
            image={imageUrl}
            alt={product.name}
            style={{ width: '100%', height: 200, objectFit: 'cover' }}
          />
          <CardContent>
            <Typography variant="h5" component="div">{product.name}</Typography>
            <Typography variant="body1" color="textSecondary">Стоимость: {product.bonus} бонусов</Typography>
            <Typography variant="body2" style={{ marginTop: '10px' }}>Описание: {product.description}</Typography>
            { role != null && (
                <TextField
                    label="Количество"
                    type="number"
                    variant="outlined"
                    size="medium"
                    value={product.quantity}
                    onChange={(e) => onQuantityChange(product.id, e.target.value)}
                    style={{ marginTop: '20px', width: '100%' }}
                    />
            )}            
          </CardContent>
        </Card>
    );
  };

const Products = () => {
    const { email, token, role } = useSelector((state) => state.auth);
    const [userBalance, setUserBalance] = useState(0);
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(true);
    const [selectedDiscountId, setSelectedDiscountId] = useState(null);
    const [selectedDiscountBonus, setSelectedDiscountBonus] = useState(null);
    const [submitLoading, setSubmitLoading] = useState(false);
    const [submitResult, setSubmitResult] = useState(null);
    const [products, setProducts] = useState([]);
    const [totalBonus, setTotalBonus] = useState(0);
    const { social } = useSelector((state) => state.social);
    //
    const dispatch = useDispatch();
    const navigate = useNavigate();

    useEffect(() => {
        const fetchProducts = async () => {
          try {
            //запрос 1 = список товаров
            const response1 = await fetch('https://localhost:7158/product/get-products', {
              method: 'GET',
              headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',
              },
            });
            //
            if (!response1.ok) {
                //редирект
                if (response1.status === 401) {
                    dispatch(logout());
                    navigate('/login');
                }
            }

            const data1 = await response1.json();
            const productsWithQuantity = data1.map(product => ({ ...product, quantity: 0 }));
            setProducts(productsWithQuantity);

            //запрос 2 = баланс пользователя
            const response2 = await fetch('https://localhost:7158/user/get-account-balance', {
              method: 'GET',
              headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',
              },
            });
    
            if (!response2.ok) {
                //редирект
                if (response2.status === 401) {
                    dispatch(logout());
                    navigate('/login');
                }
            }
            //
            const data2 = await response2.json();
            setUserBalance(data2.bonus);
          } catch (error) {
            console.error('Error:', error);
          } finally {
            setLoading(false);
          }
        };
        fetchProducts();
      }, [token]);
    
      const handleQuantityChange = (id, quantity) => {
        const updatedProducts = products.map(product =>
            product.id === id ? { ...product, quantity: parseInt(quantity, 10) } : product
        );
        setProducts(updatedProducts);
        const total = updatedProducts.reduce((sum, product) => sum + (product.quantity * product.bonus), 0);
        setTotalBonus(total);
      };
    
    const handleSelect = (id, bonus) => {
        setSelectedDiscountId(id);//храним выбранный id карты
        setSelectedDiscountBonus(bonus);//храним количество бонусов, которое стоит купон
    };

    //раздел
    const mainFeaturedPost = {
        title: 'Наши товары',
        description: "",
        image: image,
        imageText: 'main image description',
        linkText: '',
    };

    //информация о странице
    const sidebar = {
    title: 'Товары',
    description:
        'Спасибо, что выбираете товары нашего эко-бренда и поддерживаете наш проект. Вместе мы можем сделать мир чище и безопаснее! Если у вас возникли вопросы или нужны рекомендации по выбору товаров, пожалуйста, свяжитесь с нами:',    
    social: social,
    };

    //собрать данные из элементов и отправить запрос на сервер
    const handleSubmit = async (event) => {
        event.preventDefault();
        setSubmitLoading(true);

        //выбранное количество товаров
        const selectedProducts = [];
        for(const product of products) {
            if (product.quantity > 0) 
                selectedProducts.push({ id: product.id, quantity: product.quantity });
        }
        //
        const request = {
            email: email,
            products: selectedProducts
        };
    
        try {
          const response = await fetch('https://localhost:7158/product/register-product-reserve', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(request),
          });
    
          if (!response.ok) {
            throw new Error('Network response was not ok');
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
                    setUserBalance(balanceResponse.bonus);
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
    const selectedProducts = products.filter(product => product.quantity > 0);

    //рендер
    if (loading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
                <CircularProgress />
            </Box>
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
                    <Paper elevation={3} style={{ padding: '16px', marginTop: '16px', height: '100%', overflow: 'hidden' }}>
                        <Box sx={{ textAlign: 'justify', mt: 2, mb: 2, height: '100%', overflow: 'hidden' }}>
                            <Typography variant="h4" component="h1" gutterBottom>
                                Список товаров
                            </Typography>
                            <Typography variant="body1" paragraph>
                                Посмотрите наши прекрасные товары! Теперь у нас есть шопперы, худи, футболки и кепки - все с логотипом 
                                нашего экологического бренда. Они изготовлены из натуральных тканей, и самое классное - их можно обменять 
                                на бонусы для всех участников нашего экологического проекта!
                            </Typography>
                            <Typography variant="body1" paragraph>
                                Наши товары прекрасно дополнят любой гардероб благодаря своим базовым цветам - белому, бежевому и черному. 
                                Не упустите шанс добавить стильные и экологически чистые вещи в свою коллекцию!    
                            </Typography>            
                            <Grid container direction="row" spacing={2}> {}
                                {products.map((product) => (
                                <Grid item xs={12} md={6} key={product.id}> {/* для экранов меньших (xs) один элемент в строке, для экранов больших (md) два элемента в строке */}
                                    <ProductCard product={product} onQuantityChange={handleQuantityChange} role={role} />
                                </Grid>
                                ))}
                            </Grid>
                            { role != null && (
                                <>
                                    <Divider style={{ marginTop: '32px' }} />
                                    <Box sx={{ textAlign: 'justify', mt: 2, mb: 2, height: '100%', overflow: 'hidden' }}>
                                        <Typography variant="h4" component="h1" gutterBottom>
                                            Итого
                                        </Typography>
                                    </Box>
                                    {selectedProducts.length > 0 && (
                                        <Box sx={{ mt: 2, mb: 2 }}>
                                            {selectedProducts.map((product) => (
                                                <Typography key={product.id} variant="body2" color="textSecondary">
                                                    {product.name} {product.quantity} {getCorrectForm(product.quantity, ['штука', 'штуки', 'штук'])} = {product.quantity * product.bonus} бонусов
                                                </Typography>
                                            ))}
                                        </Box>
                                    )}                            
                                    <Grid container spacing={2} mb={2}>
                                        <Grid item xs={6} mb={2}>
                                            <TextField
                                                label='Стоимость'
                                                value={totalBonus}
                                                variant="outlined"
                                                fullWidth
                                                InputProps={{
                                                    readOnly: true,
                                                }}
                                                margin="normal"
                                            />
                                        </Grid>
                                        <Grid item xs={6} mb={2}>
                                            <TextField
                                                label='Кол-во бонусов'
                                                value={userBalance}
                                                variant="outlined"
                                                fullWidth
                                                InputProps={{
                                                    readOnly: true,
                                                }}
                                                margin="normal"
                                            />                                    
                                        </Grid>                                
                                    </Grid>
                                    {totalBonus > 0 && (
                                        <Typography variant="body1" paragraph>
                                        Будет списано {totalBonus} бонусов. Баллов на счете
                                        <Box component="span" sx={{ color: userBalance >= totalBonus ? 'inherit' : 'red' }}>
                                            {userBalance >= totalBonus ? ' достаточно' : ' не достаточно'}.
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
                                    {submitResult && (
                                        <Box sx={{ mt: 2 }}>
                                            <Alert severity={submitResult.success ? 'success' : 'error'}>
                                                {submitResult.message}
                                            </Alert>
                                        </Box>
                                    )}
                                </>
                            )}                        
                        </Box>
                    </Paper>
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

export {Products};