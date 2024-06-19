import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import { useNavigate } from 'react-router-dom';
import React, { useEffect, useState } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import image from '../images/about.jpg';
import CircularProgress from '@mui/material/CircularProgress';
import Sidebar from '../components/sidebar';
import MainFeaturedPost from '../components/mainFeaturedPost';
import Grid from '@mui/material/Grid';
import Divider from '@mui/material/Divider';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
} from '@mui/material';
import ImageList from '@mui/material/ImageList';
import ImageListItem from '@mui/material/ImageListItem';
import eco_3 from '../images/eco_3.jpg';
import eco_4 from '../images/eco_4.jpg';
import eco_5 from '../images/eco_5.jpg';
import eco_6 from '../images/eco_6.jpg';
import eco_7 from '../images/eco_7.jpg';
import eco_8 from '../images/eco_8.jpg';
import eco_9 from '../images/eco_9.jpg';
import eco_10 from '../images/eco_10.jpg';
import eco_11 from '../images/eco_11.jpg';
import eco_12 from '../images/eco_12.jpg';
import eco_13 from '../images/eco_13.jpg';
import eco_14 from '../images/eco_14.jpg';

function srcset(image, size, rows = 1, cols = 1) {
  return {
    src: `${image}?w=${size * cols}&h=${size * rows}&fit=crop&auto=format`,
    srcSet: `${image}?w=${size * cols}&h=${
      size * rows
    }&fit=crop&auto=format&dpr=2 2x`,
  };
}

const About = () => {
  const { social } = useSelector((state) => state.social);
  const currentRoute = useSelector(state => state.router.currentRoute);

  //раздел
  const mainFeaturedPost = {
    title: 'О нас',
    description: "",
    image: image,
    imageText: '',
    linkText: '',
  };

  //информация о странице
  const sidebar = {
      title: '',
      description: 'Мы всегда рады общению и готовы помочь вам! Если у вас есть вопросы или предложения, свяжитесь с нами',    
      social: social
  };

  const images = [
    {
      img: eco_3,
      title: 'Bed',
      rows: 2,
      cols: 2,
    },
    {
      img: eco_4,
      title: 'Books',
    },
    {
      img: eco_5,
      title: 'Sink',
    },
    {
      img: eco_6,
      title: 'Kitchen',
      cols: 2,
    },
    {
      img: eco_7,
      title: 'Blinds',
      cols: 2,
    },
    {
      img: eco_8,
      title: 'Chairs',
      rows: 2,
      cols: 2,
    },
    {
      img: eco_9,
      title: 'Laptop',
    },
    {
      img: eco_10,
      title: 'Doors',
    },
    {
      img: eco_11,
      title: 'Coffee',
      rows: 2,
      cols: 2,
    },
    {
      img: eco_12,
      title: 'Storage',
    },
    {
      img: eco_13,
      title: 'Candle',
    },
    {
      img: eco_14,
      title: 'Coffee table',
      cols: 2,
    },
  ];
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
              <Paper elevation={3} style={{ padding: '16px', marginTop: '16px' }}>
                <ImageList
                  sx={{ height: 375 }}
                  variant="quilted"
                  cols={4}
                  rowHeight={121}
                >
                  {images.map((item) => (
                    <ImageListItem key={item.img} cols={item.cols || 1} rows={item.rows || 1}>
                      <img
                        {...srcset(item.img, 121, item.rows, item.cols)}
                        alt={item.title}
                        loading="lazy"
                      />
                    </ImageListItem>
                  ))}
                </ImageList>
                <Typography variant="body1" sx={{ textAlign: 'justify' }}>
                  Добро пожаловать в сердце экологической революции! Мы - команда страстных защитников природы, стремящихся к гармонии между человеком и окружающей средой. Наш проект по переработке опасных отходов первого и второго классов опасности - это наш ответ на вызовы нашего времени, выраженный в языке заботы, ответственности и инноваций.
                </Typography>
                <Typography variant="body1" sx={{ textAlign: 'justify', mt: 2, mb: 2 }}>
                  По данным Всемирной организации здравоохранения (ВОЗ), каждый год в мире производится более 400 миллионов тонн опасных отходов. Это включает в себя отходы от промышленности, здравоохранения, сельского хозяйства, а также бытовые отходы. Опасные отходы могут содержать токсичные вещества, которые могут наносить вред здоровью людей и окружающей среде.
                </Typography>
                <Typography variant="body1" sx={{ textAlign: 'justify', mt: 2, mb: 2 }}>
                Мы верим, что каждый маленький шаг в сторону устойчивого потребления, переработки и утилизации отходов приближает нас к чистому и светлому будущему. Наша миссия не только в том, чтобы преображать опасные отходы в безопасные ресурсы, но и вдохновлять людей на заботу о драгоценной планете, которую мы делим.
                </Typography>
                <Typography variant="body1" sx={{ textAlign: 'justify', mt: 2, mb: 2 }}>
                  Присоединяйтесь к нам в этом удивительном путешествии экологического созидания! Давайте вместе создадим мир, где каждый акт заботы о природе станет каплей великого океана экологического процветания. Помните: наш дом - это Земля, и только вместе мы можем защитить его и сохранить для будущих поколений.
                </Typography>
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

export {About};