import { Link, Outlet } from 'react-router-dom';
import Box from '@mui/material/Box';
import React, { useState, useEffect } from 'react';
//import Auth from './Auth';

const Layout = () => {
    return (
        <div>
            <>
            <Box display="flex" flexDirection="column" alignItems="stretch" padding={1}>
                <header className="App-header">                
                    <Box display="flex" flexDirection="column" alignItems="stretch" padding={1}>
                        <Link to="/">Главная страница</Link>
                        <Link to="/home">Home</Link>
                        <Link to="/callback">Callback</Link>
                    </Box>
                </header>
                <main className="App-main">
                    <Outlet/>
                </main>            
                <footer className="App-footer">
                    2024
                </footer>
            </Box>
            </>            
        </div>
    );
};

export {Layout};