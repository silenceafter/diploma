import { useEffect, useState } from 'react';
import { UserManager } from 'oidc-client';
import './App.css';
import { ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
//import theme from './theme'; // Ваш файл темы Material-UI
import React from 'react';
import { connect } from 'react-redux';
import 'bootstrap/dist/css/bootstrap.min.css';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { Home } from './pages/Home';
import { Callback } from './pages/Callback';
import { Layout } from './components/Layout';

function App() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    
    /*useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch(`https://localhost:7086/Identity/Account/Login?client_id=1&redirect_uri=${window.location.origin}`, {
                    method: 'GET',
                    mode: 'no-cors'
                });
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                //
                const responseData = await response.json();
                setData(responseData);
                setLoading(false);
            } catch (error) {
                setError(error);
                setLoading(false);
            }
        };

        fetchData();
    }, []);*/

    return (
      <>
        <Router>
            <div className="App">
                <Routes>
                    <Route exact path="/" element={<Layout />} />
                    <Route path="/home" element={<Home />} />
                    <Route path="/callback" element={<Callback />} />
                </Routes>
            </div>
        </Router>
      </>
    );
}

export default App;