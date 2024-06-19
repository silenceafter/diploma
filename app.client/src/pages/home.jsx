import React from 'react';
import Main from '../components/main';
import Sidebar from '../components/sidebar';
import MainFeaturedPost from '../components/mainFeaturedPost';
import FeaturedPost from '../components/featuredPost';
import WhatsAppIcon from '@mui/icons-material/WhatsApp';
import TelegramIcon from '@mui/icons-material/Telegram';
import YoutubeIcon from '@mui/icons-material/YouTube';
import { useSelector, useDispatch } from 'react-redux';
import { Container, Box, Grid, Typography } from '@mui/material';
import RecyclingIcon from '@mui/icons-material/Recycling';
import AttachMoneyIcon from '@mui/icons-material/AttachMoney';
import DiscountIcon from '@mui/icons-material/Discount';
import Button from '@mui/material/Button';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
} from '@mui/material';
import Divider from '@mui/material/Divider';
import image from '../images/nature_3.jpg';
import photo from '../images/eco_5.jpg';
import { styled } from '@mui/system';

// Импортируйте логотипы компаний
import MagnitLogo from '../images/partner_1.png';
import LentaLogo from '../images/partner_2.png';
import PyatLogo from '../images/partner_3.png';

const Image11 = styled('img')({
  width: '100%',
  height: 'auto',
  borderRadius: 8,
});


const items = [
  {
    icon: <RecyclingIcon style={{ fontSize: 50, color: 'green' }} />,
    title: 'Сдавай отходы',
    description: 'Приноси использованные батарейки, аккумуляторы, лампочки, ртутные градусники и другие отходы (смотри список отходов, которые мы принимаем) в наши пункты приема.'
  },
  {
    icon: <AttachMoneyIcon style={{ fontSize: 50, color: 'gold' }} />,
    title: 'Получай бонусы',
    description: 'Бонусные баллы можно обменять на скидочные купоны, которые действуют в магазинах наших партнеров. Покупки могут быть еще более выгодными!'
  },
  {
    icon: <DiscountIcon style={{ fontSize: 50, color: 'purple' }} />,
    title: 'Выбирай купон на скидку',
    description: 'Список наших партнеров продолжает увеличиваться. Вы сможете найти для себя самые разные товары, а скидка сделает покупку еще более приятной! '
  }
];

const mainFeaturedPost = {
  title: 'Эко-проект по утилизации опасных отходов',
  description:
    "Эффективная координация деятельности по утилизации отходов первого и второго классов опасности",
  image: image,
  imageText: 'main image description',
  linkText: '',
};

const Home = () => {
  const { social } = useSelector((state) => state.social);
  const sidebar = {
    title: 'О нас',
    description:
      'Наш проект по обработке опасных отходов первого и второго классов опасности - это инициатива, посвящённая охране окружающей среды, созданию устойчивой экосистемы и обеспечению безопасности нашего будущего! Присоединяйтесь к нам в нашем стремлении сделать мир зеленым, чистым и безопасным для всех живых существ!',
    social: social
  };
  //
  return (
      <>
        <MainFeaturedPost post={mainFeaturedPost} />        
        <Grid container spacing={5} sx={{ mt: 3 }}>
          {<Box sx={{ textAlign: 'center', py: 5 }}>
            <Typography variant="h4" gutterBottom>
              Принимаем отходы
            </Typography>
            <Typography variant="subtitle1" gutterBottom sx={{ fontWeight: 'bold' }}>
              В 2024 году мы открыли 4 пункта по приему отходов в Курске
            </Typography>
            <Box sx={{ mt: 3, mb: 5 }}></Box>
              <Grid container spacing={4} justifyContent="center">
                {items.map((item, index) => (
                <Grid item xs={12} sm={6} md={4} key={index}>
                  <Box sx={{ textAlign: 'center', px: 3 }}>
                    {item.icon}
                    <Typography variant="h6" sx={{ mt: 2, mb: 1 }}>
                      {item.title}
                    </Typography>
                    <Typography variant="body1" color="textSecondary">
                      {item.description}
                    </Typography>
                  </Box>
                </Grid>
                ))} 
              </Grid>
          </Box>}

          <Box sx={{ p: 4 }}>
      <Grid container spacing={2} alignItems="center">
        <Grid item xs={12} md={8}>
          <Typography variant="h4" component="h1" gutterBottom sx={{ mt: 3, textAlign: 'center' }}>
            Наши планы
          </Typography>
          <Typography variant="body1">
            Мы не собираемся останавливаться на достигнутом. В ближайшем будущем мы планируем:  
          </Typography>
          <ul>
            <li>Расширить функциональность нашего веб-приложения.</li>
            <li>Увеличить число партнеров и доступных бонусов для наших пользователей.  </li>
            <li>Проводить образовательные кампании для повышения осведомленности о важности правильной утилизации опасных отходов.  </li>
          </ul>
  
          <Typography sx={{ mt: 3, textAlign: 'left' }}>Присоединяйтесь к нам  
            Мы приглашаем вас стать частью нашего сообщества и вместе с нами вносить вклад в охрану окружающей среды. Зарегистрируйтесь на нашем сайте, используйте наши услуги и помогите сделать мир чище и безопаснее.
          </Typography>
        </Grid>
        <Grid item xs={12} md={4}>
          <Paper elevation={3}>
            <Image11 src={photo} alt="Dashboard example" />
          </Paper>
        </Grid>
      </Grid>
    </Box>
    <Container sx={{ my: 4 }}>
        <Box sx={{ textAlign: 'center', mt: 4, mb: 4 }}>
            <Typography variant="subtitle1" gutterBottom>
                НАШИ ПАРТНЕРЫ:
            </Typography>
            <Divider sx={{ mb: 2 }} />
            <Grid container spacing={4} justifyContent="center" alignItems="center">
                <Grid item>
                    <img src={MagnitLogo} alt="magnit" style={{ height: 100 }} />
                </Grid>
                <Grid item>
                    <img src={LentaLogo} alt="lenta" style={{ height: 100 }} />
                </Grid>
                <Grid item>
                    <img src={PyatLogo} alt="pyat" style={{ height: 100 }} />
                </Grid>                  
            </Grid>
        </Box>
    </Container>   
</Grid>                                   
      </>
  );
};

export {Home};