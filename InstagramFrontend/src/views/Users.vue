<template>
    <Navbar />
    <div class="container mt-5 px-2" style="padding-top: 50px;">

        <div class="table-responsive">
            <table class="table table-responsive table-borderless">

                <thead>
                    <tr class="bg-light">
                        <th scope="col" width="5%"></th>
                        <th scope="col" width="5%">#id</th>
                        <th scope="col" width="20%">Username</th>
                        <th scope="col" width="10%">Password</th>
                        <th scope="col" width="20%">Description</th>
                        <th scope="col" class="text-end" width="10%"><span>Actions</span></th>
                        <th scope="col" class="text-end" width="10%"><span></span></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="user in users">
                        <th scope="row"><input class="form-check-input" type="checkbox"></th>
                        <td>{{ user.id }}</td>
                        <td>
                            <img :src="'https://localhost:7006' + user.userProfilePicture" class="rounded-circle"
                                height="25" width="25">
                            {{ user.username }}
                        </td>
                        <td>{{ user.password }}</td>
                        <td><span v-html="user.description"></span></td>
                        <td class="text-end">
                            <div class="d-flex">
                                <button class=" user-btn me-2" @click="changeRole(user.id)">Change role</button>
                                <button class=" user-btn" @click="deleteUser(user.id)">Delete</button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>

        </div>
        <Pager class="center-div" :currentPage="currentPage" :maxPages="maxPages" :loadData="getUsers" />
    </div>
</template>
<script>
import Navbar from '../components/Navbar.vue'
import Pager from '../components/Pager.vue'
import axios from 'axios'
export default {
    components: {
        Navbar,
        Pager
    },
    data() {
        return {
            users: [],
            maxPages: 0,
            currentPage: 1
        }
    },
    mounted() {
        this.getUsers()
    },
    methods: {
        deleteUser(id) {
            axios("https://localhost:7006/api/Users/" + id, {
                method: 'DELETE',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                }
            }).then(() => {
                this.getUsers()
            })
        }, changeRole(id) {
            axios("https://localhost:7006/api/Users/change-role/" + id, {
                method: 'GET',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                }
            }).then(res => {
                this.users = res.data.users
                this.maxPages = res.data.totalPages
                console.log(res.data)
            })
        },
        getUsers() {
            axios("https://localhost:7006/api/Users/all-users/" + this.currentPage, {
                method: 'GET',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                }
            }).then(res => {
                this.users = res.data.users
                this.maxPages = res.data.totalPages
                console.log(res.data)
            })
        }
    }

}
</script>
<style scoped>
.user-btn {
    color: whitesmoke;
    border-radius: 20%;
    border-color: whitesmoke;
    background: linear-gradient(180deg, rgba(139, 0, 255, 1) 55%, rgba(221, 149, 20, 1) 99%);
}

.center-div {
    display: flex;
    justify-content: center;
    align-items: center;
}

.search {

    top: 6px;
    left: 10px;
}

.form-control {

    border: none;
    padding-left: 32px;
}

.form-control:focus {

    border: none;
    box-shadow: none;
}

.green {

    color: green;
}
</style>