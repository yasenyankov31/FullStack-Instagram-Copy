<template>
    <div class="wrapper">
        <div class="logo">
            <img src="@/assets/insta_logo.png" alt="">
        </div>
        <div class="text-center mt-4 name">
            Instagram
        </div>

        <form class="p-3 mt-3">
            <div v-if="onSubmit" class="d-flex justify-content-center">
                <div class="spinner-border" role="status">
                </div>
            </div>
            <div v-else>
                <div class="form-field d-flex align-items-center">
                    <fa icon="user"></fa>
                    <input type="text" name="userName" id="userName" v-model="email" placeholder="Email">
                </div>
                <div class="form-field d-flex align-items-center ">
                    <fa icon="key"></fa>
                    <input type="password" name="password" id="pwd" v-model="password" placeholder="Password">
                </div>
                <div class="error">{{ onSubmitError }}</div>
                <button class="btn mt-3" v-on:click.prevent="submitForm">Login</button>
            </div>
            <div class="text-center fs-6">
                <h6>Don't have account ?</h6>
                <RouterLink :to="{ name: 'register' }">Sign up</RouterLink>
            </div>
        </form>
    </div>
</template>

<script>
import axios from 'axios'
export default {
    data() {
        return {
            email: '',
            password: '',
            onSubmit: false,
            onSubmitError: ''

        }
    },
    created() {
        axios("https://localhost:7006/api/Posts", {
            method: 'get',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('token')
            }
        }).then(res => {
            if (res.status == 200) {
                this.$router.push({ path: '/' })
            }
        })
    },
    methods: {
        submitForm() {
            if (this.password === '' || this.email === '') {
                this.onSubmitError = 'Please fill the two fields!'
            }
            else {
                this.onSubmit = true
                const formData = new FormData();
                formData.append('Id', 0);
                formData.append('Username', 'name');
                formData.append('Password', this.password);
                formData.append('Email', this.email);
                formData.append('Description', '');
                formData.append('Role', 0);

                axios({
                    method: 'POST',
                    url: 'https://localhost:7006/api/Account/login',
                    data: formData,
                    headers: {
                        'Accept': " */*",
                        'Content-Type': `multipart/form-data`,
                    },
                }).then(response => {
                    localStorage.setItem('token', response.data.token)
                    localStorage.setItem('interface', response.data.role)
                    localStorage.setItem('profilePic', response.data.profilepicUrl)
                    setTimeout(() => {
                        this.$router.push({ path: '/' })
                    }, 2000);

                }).catch(error => {
                    if (error.response.status === 401) {
                        this.onSubmit = false
                        this.onSubmitError = 'Invalid credentials!'
                        this.email = ''
                        this.password = ''
                    }
                    console.log(error)
                });

            }
        }
    }
}
</script>

<style scoped>
.error {
    color: #ff0062;
    margin-top: 10px;
    font-size: 0.8em;
    font-weight: bold;
}

/* Reseting */
* {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
    font-family: 'Poppins', sans-serif;
}

body {
    background: #ecf0f3;
}

.wrapper {
    max-width: 350px;
    min-height: 500px;
    margin: 80px auto;
    padding: 40px 30px 30px 30px;
    background-color: #ecf0f3;
    border-radius: 15px;
    box-shadow: 13px 13px 20px #cbced1, -13px -13px 20px #fff;
}

.logo {
    width: 80px;
    margin: auto;
}

.logo img {
    width: 100%;
    height: 80px;
    object-fit: cover;
    border-radius: 50%;
    box-shadow: 0px 0px 3px #5f5f5f,
        0px 0px 0px 5px #ecf0f3,
        8px 8px 15px #a7aaa7,
        -8px -8px 15px #fff;
}

.wrapper .name {
    font-weight: 600;
    font-size: 1.4rem;
    letter-spacing: 1.3px;
    padding-left: 10px;
    color: #555;
}

.wrapper .form-field input {
    width: 100%;
    display: block;
    border: none;
    outline: none;
    background: none;
    font-size: 1.2rem;
    color: #666;
    padding: 10px 15px 10px 10px;
    /* border: 1px solid red; */
}

.wrapper .form-field {
    padding-left: 10px;
    margin-bottom: 20px;
    border-radius: 20px;
    box-shadow: inset 8px 8px 8px #cbced1, inset -8px -8px 8px #fff;
}

.wrapper .form-field .fas {
    color: #555;
}

.wrapper .btn {
    box-shadow: none;
    width: 100%;
    height: 40px;
    background: linear-gradient(90deg, rgba(210, 210, 75, 1) 0%, rgba(221, 149, 20, 1) 35%, rgba(139, 0, 255, 1) 100%);
    color: #fff;
    border-radius: 25px;
    box-shadow: 3px 3px 3px #b1b1b1,
        -3px -3px 3px #fff;
    letter-spacing: 1.3px;
}


.wrapper a {
    text-decoration: none;
    font-size: 0.8rem;
    color: rgba(221, 149, 20, 1);
}

.wrapper a:hover {
    color: rgba(139, 0, 255, 1);
}

@media(max-width: 380px) {
    .wrapper {
        margin: 30px 20px;
        padding: 40px 15px 15px 15px;
    }
}
</style>