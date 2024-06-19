import './app.css';
import React, { Suspense, lazy, useState, useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import CssBaseline from '@mui/material/CssBaseline';
import { Container } from '@mui/material';
import Header from './components/header';
import Footer from './components/footer';
import { Register } from './pages/register';
import { Login } from './pages/login';
import { Logout } from './pages/logout';
import { Home } from './pages/home';
import { Account } from './pages/account';
import { About } from './pages/about';
import { Partners } from './pages/partners';
import { Products } from './pages/products';
import { Points } from './pages/points';
import { AccessDenied } from './pages/accessDenied';
import { BonusExchange } from './pages/bonusExchange';
import { Acceptance } from './pages/acceptance';
import { Forgot } from './pages/forgot';
import { AdminCoupons } from './pages/admin/coupons';
import { AdminHazardWaste } from './pages/admin/hazardWaste';
import { AdminPartners } from './pages/admin/partners';
import { AdminPoints } from './pages/admin/points';
import { AdminProducts } from './pages/admin/products';
import { AdminUsers } from './pages/admin/users';

const App = () => {
  const { email, token } = useSelector((state) => state.auth);
  const dispatch = useDispatch();
  //
  return (    
    <Router>
      <div className="App">
        <CssBaseline />
          <Container maxWidth="lg">
            <Header title="SafeNature" />
            <main>
              <Routes>
                  <Route index element={<Home />} />
                  <Route path="/register" element={<Register />} />
                  <Route path="/login" element={<Login />} />
                  <Route path="/logout" element={<Logout />} />
                  <Route path="/forgot" element={<Forgot />} />
                  <Route path="/account" element={<Account />} />
                  <Route path="/about" element={<About />} />
                  <Route path="/partners" element={<Partners />} />
                  <Route path="/products" element={<Products />} />
                  <Route path="/points" element={<Points />} />
                  <Route path="/bonus-exchange" element={<BonusExchange />} />
                  <Route path="/acceptance" element={<Acceptance />} />
                  <Route path="/access-denied" element={<AccessDenied />} />
                  <Route path="/admin/coupons" element={<AdminCoupons />} />
                  <Route path="/admin/hazardWaste" element={<AdminHazardWaste />} />
                  <Route path="/admin/partners" element={<AdminPartners />} />
                  <Route path="/admin/points" element={<AdminPoints />} />
                  <Route path="/admin/products" element={<AdminProducts />} />
                  <Route path="/admin/users" element={<AdminUsers />} />
              </Routes>
            </main>
          </Container>
          <Footer
              title=""
              description=""
          />
      </div>
    </Router> 
  );
};

export default App;
