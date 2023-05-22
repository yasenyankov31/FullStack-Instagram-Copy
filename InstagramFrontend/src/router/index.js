import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import Login from '../views/AccViews/Login.vue'
import Register from '../views/AccViews/Register.vue'
import Account from '../views/AccViews/Account.vue'
import Users from '../views/Users.vue'

const routes = [
  {
    path: '/',
    name: 'home',
    component: Home
  },
  {
    path: '/login',
    name: 'login',
    component:Login
  }  ,
  {
    path: '/register',
    name: 'register',
    component:Register
  } ,
  {
    path: '/account:id',
    name: 'account',
    component:Account
  } ,
  {
    path: '/users',
    name: 'users',
    component:Users
  }
  
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
