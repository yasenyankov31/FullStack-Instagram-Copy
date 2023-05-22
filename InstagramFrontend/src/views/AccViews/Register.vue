<template>
    <section class="vh-100 backgroundclr">
        <div class="container h-100">
            <div class="row d-flex justify-content-center align-items-center h-100">
                <div class="col-lg-12 col-xl-11">
                    <div class="card text-black" style="border-radius: 25px;">
                        <div class="card-body p-md-5">
                            <div class="row justify-content-center">
                                <div class="col-md-10 col-lg-6 col-xl-5 order-2 order-lg-1">

                                    <p class="text-center h1 fw-bold mb-5 mx-1 mx-md-4 mt-4">Sign up</p>
                                    <div v-if="onSubmit" class="d-flex justify-content-center">
                                        <div class="spinner-border" role="status">
                                        </div>
                                    </div>
                                    <form v-else class="mx-1 mx-md-4">

                                        <div class="d-flex flex-row align-items-center mb-4">
                                            <i class="fas fa-user fa-lg me-3 fa-fw"></i>
                                            <div class="form-outline flex-fill mb-0">
                                                <input v-model="username" type="text" id="form3Example1c"
                                                    class="form-control" />
                                                <label class="form-label" for="form3Example1c">Username</label>
                                            </div>
                                        </div>

                                        <div class="d-flex flex-row align-items-center mb-4">
                                            <i class="fas fa-envelope fa-lg me-3 fa-fw"></i>
                                            <div class="form-outline flex-fill mb-0">
                                                <input v-model="email" type="email" id="form3Example3c"
                                                    class="form-control" />
                                                <label class="form-label" for="form3Example3c">Email</label>
                                            </div>
                                        </div>

                                        <div class="d-flex flex-row align-items-center mb-4">
                                            <i class="fas fa-lock fa-lg me-3 fa-fw"></i>
                                            <div class="form-outline flex-fill mb-0">
                                                <input v-model="password" type="password" id="form3Example4c"
                                                    class="form-control" />
                                                <label class="form-label" for="form3Example4c">Password</label>
                                            </div>
                                        </div>

                                        <div class="d-flex flex-row align-items-center mb-4">
                                            <i class="fas fa-key fa-lg me-3 fa-fw"></i>
                                            <div class="form-outline flex-fill mb-0">
                                                <input v-model="confirmpassword" type="password" id="form3Example4cd"
                                                    class="form-control" />
                                                <label class="form-label" for="form3Example4cd">Repeat your password</label>
                                            </div>
                                        </div>
                                        <div class="d-flex flex-row align-items-center mb-4">
                                            <i class="fas fa-key fa-lg me-3 fa-fw"></i>
                                            <div class="form-outline flex-fill mb-0">
                                                <input v-on:change="onFileChange" class="form-control form-control-lg"
                                                    id="formFileLg" type="file">
                                                <label class="form-label" for="form3Example4cd">Profile picture</label>
                                            </div>
                                        </div>
                                        <div class="d-flex flex-row align-items-center mb-4">
                                            <div class="error text-center flex-fill">{{ onSubmitError }}</div>
                                        </div>

                                        <div class="form-check  d-flex justify-content-center mb-5 ">
                                            <input v-on:change="acceptTerms" class="form-check-input accpetbtn me-2"
                                                type="checkbox" value="" id="form2Example3c" />
                                            <label class="form-check-label" for="form2Example3">
                                                I agree all statements in <a href="#!">Terms of service</a>
                                            </label>
                                        </div>

                                        <div class="d-flex justify-content-center mx-4 mb-3 mb-lg-4">
                                            <button type="button" class="custombtn"
                                                v-on:click.prevent="submitForm">Register</button>
                                        </div>
                                        <div class=" d-flex justify-content-center mb-5">Already have account? <RouterLink
                                                :to="{ name: 'login' }">
                                                Sign in</RouterLink>
                                        </div>
                                    </form>

                                </div>
                                <div
                                    class="col-md-10 col-lg-6 col-xl-7 d-flex align-items-center order-1 order-lg-2 d-none d-md-none d-lg-block">

                                    <img src="@/assets/instagram_img.png" class="img-fluid" alt="Sample image">

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
</template>

<script>
import axios from 'axios'
export default {
    data() {
        return {
            email: '',
            password: '',
            confirmpassword: '',
            username: '',
            profilePic: '',
            terms: false,
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
        validate() {
            if (this.confirmpassword !== this.password) {
                this.onSubmitError = 'Both passwords must be the same!'
                return false
            }
            else if (!this.terms) {
                this.onSubmitError = "Accept the terms!"
                return false;
            }
            else if (this.password.leng < 8) {
                this.onSubmitError = 'Password must be larger than 8 symbols!'
                return false
            }
            else if (this.email === '' || this.password === ''
                || this.confirmpassword === '' || this.username === '') {
                this.onSubmitError = 'All fields must be field (profile picture is not mendatory)!'
                return false
            }
            return true
        },
        submitForm() {
            if (this.validate()) {
                const formData = new FormData();
                formData.append('Id', 0);
                formData.append('Username', this.username);
                formData.append('Password', this.password);
                formData.append('Role', 0);
                formData.append('Description', '');
                formData.append('Email', this.email);
                formData.append('Image', this.profilePic);
                this.onSubmit = true

                axios({
                    method: 'POST',
                    url: 'https://localhost:7006/api/Account/register',
                    data: formData,
                    headers: {
                        'Accept': " */*",
                        'Content-Type': `multipart/form-data`,
                    },
                }).then(response => {
                    console.log(response)
                    localStorage.setItem('token', response.data.token)
                    localStorage.setItem('interface', response.data.role)
                    localStorage.setItem('profilePic', response.data.profilepicUrl)
                    setTimeout(() => {
                        this.$router.push({ path: '/' })
                    }, 2000);
                }).catch(error => {
                    this.onSubmitError = 'Something went wrong!'
                    console.log(error)
                });
            }


        },
        onFileChange(event) {
            this.profilePic = event.target.files[0];
        },
        acceptTerms() {
            this.terms = !this.terms
        }
    }
}
</script>
<style>
.error {
    color: #ff0062;
    margin-top: 10px;
    font-size: 0.8em;
    font-weight: bold;
}

.backgroundclr {
    background: linear-gradient(90deg, rgba(139, 0, 255, 1) 0%, rgba(221, 149, 20, 1) 52%, rgba(42, 41, 41, 1) 93%);
}

.custombtn {
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

.accpetbtn:checked {
    background-color: rgba(221, 149, 20, 1) !important;
    border-color: rgba(139, 0, 255, 1) !important;
}
</style>