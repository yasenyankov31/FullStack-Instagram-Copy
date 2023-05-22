<template>
  <router-view />
</template>

<script>
import axios from 'axios'
export default {
  
  mounted() {
    axios("https://localhost:7006/api/Account/isAuth", {
      method: 'get',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + localStorage.getItem('token')
      }
    }
    ).catch(res => {
      if (res.response.status == 401) {
        this.$router.push({ path: '/login' })
      }
    })
  }
};
</script>

