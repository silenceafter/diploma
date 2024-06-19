import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import { useNavigate } from 'react-router-dom';
import React, { useEffect, useState } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import image from '../images/partners.jpg';

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

const Partners = () => {
  const [userData, setUserData] = useState(null);
  const [loading, setLoading] = useState(true);
  const { email, token } = useSelector((state) => state.auth);
  const { social } = useSelector((state) => state.social);
  const dispatch = useDispatch();
  const navigate = useNavigate();  
  
  //раздел
  const mainFeaturedPost = {
      title: 'Магазины',
      description: "",
      image: image,
      imageText: '',
      linkText: '',
  };

  //информация о странице
  const sidebar = {
      title: '',
      description:
          'Обменивайте бонусные баллы на скидочные купоны в магазинах-партнерах. Список магазинов, которые принимают участие в программе обмена.',    
      social: social
  };
  //
  useEffect(() => {
      const userRequest = async () => {
      try {
          const response = await fetch('https://localhost:7158/partner/getpartners', {
              method: 'GET',
              headers: {
                'Content-Type': 'application/json'
              }
          });

          if (response.ok) {
              setUserData(await response.json());
          } else {                                            
          }
          } catch (error) {
              console.error('Error:', error);
          } finally {
              setLoading(false);
          }
      };
      userRequest();
  }, []);

  //рендер
  if (loading) {
      return (
          <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
              <CircularProgress />
          </Box>
      );
  }

  //добавляем строки в таблицу
  const tableRows = [];
  for (let i = 0; i < userData.length; i++) {
    tableRows.push(
      <TableRow>
        <TableCell>{i + 1}</TableCell>
        <TableCell>{userData[i].name}</TableCell>
        <TableCell>{userData[i].description}</TableCell>
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
              <TableContainer component={Paper}>
                <Table aria-label="user table">
                  <TableHead>
                    <TableRow>
                      <TableCell>#</TableCell>
                      <TableCell>Название</TableCell>
                      <TableCell>Описание</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>{tableRows}</TableBody>
                </Table>
              </TableContainer>
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

export {Partners};