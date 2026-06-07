import React, { useContext, useEffect, useState } from 'react';
import { AuthContext } from '../../context/AuthContext';
import apiClient from '../../services/apiClient';


import './Dashboard.css';
import NavigationPanel from './NavigationPanel';
import ProductsTable from './ProductsTable';

const Dashboard = () => {
    const { user, logout } = useContext(AuthContext);

    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        apiClient.get('/api/Products')
            .then(res => {
                setProducts(res.data);
                setLoading(false);
            })
            .catch(err => {
                console.error(err);
                setLoading(false);
            });
    }, []);

    return (
        <div className="dashboard-container">

            <NavigationPanel user={user} />

            <section className="products-section">
                <div className="section-header">
                    <h2>Product Catalog</h2>
                </div>

                <ProductsTable
                    products={products}
                    loading={loading}
                />
            </section>

        </div>
    );
};

export default Dashboard;