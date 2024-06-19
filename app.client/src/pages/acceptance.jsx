import Typography from '@mui/material/Typography';
import { useSelector, useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import React, { useEffect, useState } from 'react';
import Sidebar from '../components/sidebar';
import MainFeaturedPost from '../components/mainFeaturedPost';
import image from '../images/acceptance.jpg';
import { Divider } from '@mui/material';
import { Grid, TextField, Box  } from '@mui/material';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import FormControl from '@mui/material/FormControl';
import Select from '@mui/material/Select';
import Button from '@mui/material/Button';
import AddIcon from '@mui/icons-material/Add';
import RemoveIcon from '@mui/icons-material/Remove';
import CircularProgress from '@mui/material/CircularProgress';
import { logout } from '../store/actions/authActions';
import Alert from '@mui/material/Alert';

const Acceptance = () => {
    const { email, token } = useSelector((state) => state.auth);
    

    //отходы
    const [fields, setFields] = useState([
        { id: 1, selectValue: '', textFieldQuantityValue: 1, textFieldBonusValue: 0 }
    ]);
    const [userData, setUserData] = useState(null);
    const [userLoading, setUserLoading] = useState(true);

    //point, пункт приема
    const [point, setPoint] = useState('');
    const [pointData, setPointData] = useState(null);    
    const [pointLoading, setPointLoading] = useState(true);
    
    const [error, setError] = useState(null);
    const [submitLoading, setSubmitLoading] = useState(false);//для submit
    const [submitResult, setSubmitResult] = useState(null);
    const { social } = useSelector((state) => state.social);
    const dispatch = useDispatch();
    const navigate = useNavigate();

    //раздел
    const mainFeaturedPost = {
        title: 'Принять отходы',
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

    //рассчитаем бонусы
    const calculateBonus = (selectValue, quantityValue) => {
        const selectedItem = userData.find(item => item.id === selectValue);
        return selectedItem ? selectedItem.bonuses * quantityValue : 0;
    };

    //изменить пункт приема отходов
    const handlePointChange = (event) => {
        setPoint(event.target.value);
    };

    //добавить строку
    const handleAddField = () => {
        setFields([...fields, { id: fields.length + 1, selectValue: '', textFieldQuantityValue: 1, textFieldBonusValue: 0 }]);
    };

    //удалить строку
    const handleRemoveField = (id) => {
        setFields(fields.filter((field) => field.id !== id));
    };

    //изменить значение Select
    const handleSelectChange = (id, newValue) => {
        const field = fields.find(field => field.id === id);
        const quantityValue = field ? field.textFieldQuantityValue : 0;
        //
        setFields(fields.map((field) =>
          field.id === id ? { ...field, selectValue: newValue, textFieldBonusValue: calculateBonus(newValue, quantityValue) } : field
        ));
    };
    
    //изменить значение TextValue1
    const handleTextFieldQuantityChange = (id, newValue) => {
        const field = fields.find(field => field.id === id);
        const selectValue = field ? field.selectValue : '';

        setFields(fields.map((field) =>
            field.id === id ? { ...field, textFieldQuantityValue: newValue, textFieldBonusValue: calculateBonus(selectValue, newValue) } : field
        ));
    };

    //собрать данные из элементов и отправить запрос на сервер
    const handleSubmit = async (event) => {
        event.preventDefault();
        setSubmitLoading(true);

        //данные запроса
        const wasteItems = [];
        for(const field of fields)
            wasteItems.push({ hazardousWasteId: field.selectValue, quantity: field.textFieldQuantityValue });
        //
        const request = {
            pointId: point,
            email: email,
            wasteItems: wasteItems
        }
    
        try {
          const response = await fetch('https://localhost:7158/acceptance/register-dispose', {
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
          setFields([{ id: 1, selectValue: '', textFieldQuantityValue: 1, textFieldBonusValue: 0 }]);//оставить первую строку, очистить элементы
          setPoint('');
        } catch (error) {
          console.error('Error:', error);
          setSubmitResult({ success: false, message: 'Ошибка при отправке данных.' });
        } finally {
            setSubmitLoading(false);
            setTimeout(() => setSubmitResult(null), 3000);
        }
      };  

    //useEffect
    useEffect(() => {
        //доступ запрещен
        if (!email)
            return;//navigate('/access-denied');
        
        //запросы: 1-й = получить список отходов
        const userRequest = async () => {
            try {
                setUserLoading(true);
                const response = await fetch('https://localhost:7158/hazardouswaste/get-hazardous-waste', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });
                //
                if (response.ok) {
                    const userResponse = await response.json();
                    setUserData(userResponse);

                    //2-й запрос = получить список пунктов сдачи
                    setPointLoading(true);
                    const response2 = await fetch('https://localhost:7158/point/get-points', {
                        method: 'GET',
                        headers: {
                            'Authorization': `Bearer ${token}`
                        }
                    });                    
                    //
                    if (response2.ok) {
                        const pointResponse = await response2.json();
                        setPointData(pointResponse);
                    }
                    setPointLoading(false);
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
                setUserLoading(false);
            }
        };
        userRequest();
        return () => 
            // Очистка таймера при размонтировании компонента
            clearTimeout();
    }, []);

    //рендер
    //доступ запрещен
    if (!email)
        navigate('/access-denied');
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
                                Список отходов
                            </Typography>
                            <Typography variant="body1" paragraph>
                                Добавляйте отходы из списка, затем нажмите "Подтвердить".
                            </Typography>            
                            <Box
                                component="form"
                                onSubmit={handleSubmit}
                                sx={{
                                    display: 'flex',
                                    flexDirection: 'column',
                                    alignItems: 'justify',
                                    justifyContent: 'center',
                                    position: 'relative',
                                    width: '100%',
                                    bgcolor: 'background.paper',
                                    p: 4,
                                    borderRadius: 1,
                                    boxShadow: 3,
                                    pl: 3.5,
                                    pr: 6
                                }}
                            >
                                <FormControl fullWidth required sx={{ marginBottom: 2 }}>
                                    <InputLabel id="select-label-point">Пункт приема</InputLabel>
                                    <Select
                                        labelId="select-label-point"
                                        value={point}
                                        label="Пункт приема"
                                        onChange={handlePointChange}
                                        disabled={pointLoading}
                                    >
                                        <MenuItem value="" disabled>Выберите пункт приема</MenuItem>
                                        {pointData && pointData.length > 0 ? (
                                        pointData.map((option) => (
                                            <MenuItem key={option.id} value={option.id}>
                                            {option.name}, {option.address}
                                            </MenuItem>
                                        ))
                                        ) : (
                                        <MenuItem disabled>Нет доступных данных</MenuItem>
                                        )}
                                    </Select>
                                </FormControl> 

                                {fields.map((field, index) => (
                                    <Grid container spacing={2} alignItems="center" key={field.id}>
                                    <Grid item xs={6}>
                                        <FormControl required sx={{ mb: 2 }} fullWidth>
                                            <InputLabel id={`select-label-${field.id}`}>Отход</InputLabel>
                                            <Select                                            
                                                labelId={`select-label-${field.id}`}
                                                value={field.selectValue}
                                                label="Отход"
                                                onChange={(event) => handleSelectChange(field.id, event.target.value, 0)}
                                                disabled={userLoading}
                                            >
                                                {userData && userData.length > 0 ? (
                                                    userData.map((option) => (
                                                        <MenuItem key={option.id} value={option.id}> 
                                                            {option.name}
                                                        </MenuItem>
                                                    ))
                                                ) : (
                                                    <MenuItem disabled>Нет доступных данных</MenuItem>
                                                )}
                                            </Select>                                            
                                        </FormControl>
                                    </Grid>
                                    <Grid item xs={2} mb={2}>
                                        <TextField
                                            label="Кол-во"
                                            type="number"
                                            value={field.textFieldQuantityValue}
                                            onChange={(event) => handleTextFieldQuantityChange(field.id, parseInt(event.target.value, 10) || 0)}
                                            />
                                    </Grid>
                                    <Grid item xs={2} mb={2}>
                                        <TextField
                                            readOnly={true}
                                            label="Бонусы"
                                            type="number"
                                            value={field.textFieldBonusValue}                                           
                                        />
                                    </Grid>
                                    <Grid item xs={2} mb={2}>
                                        <Button
                                        variant="contained"
                                        color="primary"
                                        startIcon={<RemoveIcon />}
                                        onClick={() => handleRemoveField(field.id)}
                                        disabled={fields.length === 1} //не позволяем удалить последний оставшийся элемент
                                        >
                                        Удалить
                                        </Button>
                                    </Grid>
                                    </Grid>
                                ))}
                                <Box sx={{ mt: 2 }}>
                                    <Button
                                    variant="contained"
                                    color="primary"
                                    startIcon={<AddIcon />}
                                    onClick={handleAddField}
                                    >
                                    Добавить
                                    </Button>
                                </Box>
                                <Box sx={{ mt: 1 }}>
                                    <Button                            
                                    variant="contained"
                                    color="secondary"
                                    type="submit"
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

export {Acceptance};