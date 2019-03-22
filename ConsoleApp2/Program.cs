using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            hello();
            Console.ReadKey();
        }
        static void hello() //基础对话面
        {
            Console.WriteLine("欢迎使用段浩彬的新闻系统");
            Console.WriteLine("请问你的身份是？");
            Console.WriteLine("【1】用户    【2】管理员");
            string identity = Console.ReadLine();
            if (identity == "1")
                UserStart();
            else if (identity == "2") AdminStart();
            else hello();
        }
        static void UserStart()//user登陆
        {
            Console.WriteLine("噢！原来是尊贵的用户");
            Console.WriteLine("请输入你的用户名：");
            string username = Console.ReadLine();
            Console.WriteLine("请输入你的密码：");
            string password = Console.ReadLine();
            if (VerifyUser(username, password))//用先用root登陆，再认证身份
            {
                Console.WriteLine("成功连接database_newssystem数据库");
                Console.WriteLine("欢迎{0}", username);
            }
            else
            {
                Console.WriteLine("连接失败，请重新输入");
                UserStart();
            }
            SeeNews(username, password);

        }
        static void SeeNews(string username, string password)
        {
            Console.WriteLine("请输入关键字");
            string keyword = '%' + Console.ReadLine() + '%';
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=" + username + ";password=" + password + "";
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                int flag = 0;
                string sql = "select *  from news where newsTitle like '" + keyword + "'"; //我们自己按照查询条件组拼mysql 很麻烦
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                MySqlDataReader reader = command.ExecuteReader();
                //reader.Read();  //调用一次相当于翻一页书


                //Console.WriteLine(reader[0].ToString() + reader[1].ToString() + reader[2].ToString());


                while (reader.Read()) //如果有数据 返回True 没数据返回false
                {
                    //Console.WriteLine(reader[0].ToString() + reader[1].ToString() /*+ reader[2].ToString()*/);
                    flag = 1;//表示能搜到相关内容

                    //Console.WriteLine( reader.GetInt32(0)+ " "+reader.GetString(1)+ " "+reader.GetString(2));
                    Console.WriteLine(reader.GetString("newsID") + " " + reader.GetString("newsTitle"));
                }
                //command.ExecuteReader(); //执行一些查询
                //command.ExecuteNonQuery(); //插入删除
                //command.ExecuteScalar();//执行一些查询，返回一个单个值
                if (flag == 0)
                {
                    Console.WriteLine("对不起，整个数据库翻了个遍都没有相关标题的新闻");
                    Console.WriteLine("请您尝试使用其他关键词搜索");
                    SeeNews(username, password);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                conn.Close();
            }
            Console.WriteLine("请问你想查看哪一则新闻，请输入相应的编号，或者输入“0”返回");
            string num = Console.ReadLine();
            if (num == "0")
                SeeNews(username, password);
            else ViewDetail(num, username, password);

        }//真正的用用户账号密码登陆数据库
        static void ViewDetail(string num, string username, string password)
        {
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=" + username + ";password=" + password + "";
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "select *  from news where newsID=" + num + ""; //我们自己按照查询条件组拼mysql 很麻烦
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                MySqlDataReader reader = command.ExecuteReader();
                reader.Read();
                Console.WriteLine(reader.GetString("newsID") + " " + reader.GetString("newsTitle") + " " + reader.GetString("newsContent"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                conn.Close();
            }
            Console.WriteLine("留下评论请按“r”查看其他新闻请输入对应编号，返回查询请按“b”");
            string op = Console.ReadLine();
            if (op == "b") SeeNews(username, password);
            else if (op == "r") Review(num, username, password);
            else ViewDetail(op, username, password);
        }
        static int MaxNCommentID(string username, string password)
        {
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=" + username + ";password=" + password + "";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库
            int nCommentID = 0;
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "select max(nCommentID)  from news_Comment";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                object reader = command.ExecuteScalar();
                nCommentID = Convert.ToInt32(reader.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            finally
            {
                conn.Close();
            }
            return nCommentID;
        }
        static int MaxUCID()
        {
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=root;password=123456";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库
            int uCID = 0;
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "select max(uCID)  from users_Comment";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                object reader = command.ExecuteScalar();
                uCID = Convert.ToInt32(reader.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            finally
            {
                conn.Close();
            }
            return uCID;
        }
        static void Review(string num, string username, string password)
        {
            Console.WriteLine("留言区：");
            string comment = Console.ReadLine();
            int commentID = MaxCommentID(username, password);
            commentID++;
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=" + username + ";password=" + password + "";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "insert into comment(commentID,commentContent) values('" + commentID + "','" + comment + "')";
                //string sql = "insert into users(username,password,time) values('f','g','" + DateTime.Now + "')";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                command.ExecuteNonQuery();                                                    // MySqlDataReader reader = command.ExecuteReader();
                Console.WriteLine("感谢你的评论！");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                conn.Close();
            }
            int nCommentID = MaxNCommentID(username, password);
            nCommentID++;
            InsertNC(nCommentID, num, commentID);
            int userID = SelectUserID(username);
            int uCID = MaxUCID();
            uCID++;
            InsertUC(uCID, userID, commentID);
            Console.WriteLine("请问你接下来想进行什么操作？");
            Console.WriteLine("【1】返回登陆页面，【2】返回搜索界面，推出请点右上方的关闭按钮");
            string next = Console.ReadLine();
            if (next == "1") hello();
            else if (next == "2") SeeNews(username, password);

            Console.ReadKey();
        }
        static bool VerifyUser(string username, string password)
        {
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=root;password=123456";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库

            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "select *  from user where userName='" + username + "'and userPassword='" + password + "'"; //我们自己按照查询条件组拼mysql 很麻烦


                //string sql = "select *  from users where username=@username and password=@password ";


                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                //command.Parameters.AddWithValue("username", username);
                //command.Parameters.AddWithValue("password", password);


                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return false;
        }
        static int MaxCommentID(string username, string password)
        {
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=" + username + ";password=" + password + "";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库
            int commentID = 0;
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "select max(commentID)  from comment";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                object reader = command.ExecuteScalar();
                commentID = Convert.ToInt32(reader.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            finally
            {
                conn.Close();
            }
            return commentID;
        }
        static int SelectUserID(string username)
        {
            int userID = 0;
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=root;password=123456";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "select userID  from user where userName='" + username + "'";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                object reader = command.ExecuteScalar();
                userID = Convert.ToInt32(reader.ToString());
                return userID;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            finally
            {
                conn.Close();
            }
            return userID;
        }
        static void AdminStart()//Admin登陆
        {
            Console.WriteLine("哎！原来是苦逼的管理员");
            Console.WriteLine("请输入你的管理员名称：");
            string adminname = Console.ReadLine();
            Console.WriteLine("请输入你的密码：");
            string password = Console.ReadLine();
            if (VerifyAdmin(adminname, password))//用先用root登陆，再认证身份
            {
                Console.WriteLine("成功连接database_newssystem数据库");
                Console.WriteLine("欢迎{0}", adminname);
            }
            else
            {
                Console.WriteLine("连接失败，请重新输入");
                AdminStart();
            }
            AdminOperate(adminname, password);
        }
        static void AdminOperate(string adminname, string password)
        {
            Console.WriteLine("【1】查看当前所有用户信息，【2】发布新闻，【3】查看当前全部评论信息，【0】返回登陆页面");
            string op = Console.ReadLine();
            if (op == "1")
            {
                SeeUser(adminname, password);
            }
            else if (op == "2")
            {
                ReleaseNew(adminname, password);
            }
            else if (op == "3")
            {
                SeeComment(adminname, password);
            }
            else if (op == "0")
            {
                hello();
            }
        }
        static void SeeComment(string adminname, string password)
        {
            Console.WriteLine("当前所有评论数量：");
            ShowCommentNum(adminname, password);
            Console.WriteLine("newsID     COUNT()");
            Console.WriteLine("------------------------");
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=" + adminname + ";password=" + password + "";
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "select newsID,count(*) from news_Comment group by newsID"; //我们自己按照查询条件组拼mysql 很麻烦
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read()) //如果有数据 返回True 没数据返回false
                {
                    Console.WriteLine(reader.GetInt32(0) + "                 " + reader.GetString(1));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                conn.Close();
            }
            Console.WriteLine("请问你想查看哪则新闻的全部评论，请输入相应的编号，或者输入“0”返回");
            string num = Console.ReadLine();
            if (num == "0")
                AdminOperate(adminname, password);
            else
            {
                ShowAllComment(num, adminname, password);
            }
        }
        static bool VerifyAdmin(string adminname, string password)
        {
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=root;password=123456";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库

            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "select *  from admin where adminName='" + adminname + "'and adminPassword='" + password + "'"; //我们自己按照查询条件组拼mysql 很麻烦

                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令


                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return false;
        }
        static void SeeUser(string adminname, string password)
        {
            Console.WriteLine("当前所有用户数量：");
            ExcuteScalar(adminname, password);
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=" + adminname + ";password=" + password + "";
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "select userID,userName  from user "; //我们自己按照查询条件组拼mysql 很麻烦
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                MySqlDataReader reader = command.ExecuteReader();


                while (reader.Read()) //如果有数据 返回True 没数据返回false
                {

                    Console.WriteLine(reader.GetString("userID") + " " + reader.GetString("userName"));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                conn.Close();
            }
            Console.WriteLine("请问你想查看哪位用户的详细信息，请输入相应的编号，或者输入“0”返回");
            string num = Console.ReadLine();
            if (num == "0")
                AdminOperate(adminname, password);
            else
            {
                UserDetail(num, adminname, password);
            }
        }
        static void ExcuteScalar(string adminname, string password)  //返回当前用户数量 当返回值只有一个值的时候 使用ExcuteScalar 比较方便
        {
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=" + adminname + ";password=" + password + "";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                //string sql = "select id , password   from users";
                string sql = "select count(*)  from user";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                object reader = command.ExecuteScalar();
                int count = Convert.ToInt32(reader.ToString());
                Console.WriteLine(count);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        static void ShowCommentNum(string adminname, string password)  //返回当前用户数量 当返回值只有一个值的时候 使用ExcuteScalar 比较方便
        {
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=" + adminname + ";password=" + password + "";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                //string sql = "select id , password   from users";
                string sql = "select count(*)  from comment";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                object reader = command.ExecuteScalar();
                int count = Convert.ToInt32(reader.ToString());
                Console.WriteLine(count);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        static void UserDetail(string num, string adminname, string password)
        {
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=" + adminname + ";password=" + password + "";
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "select *  from user where userID=" + num + ""; //我们自己按照查询条件组拼mysql 很麻烦
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                MySqlDataReader reader = command.ExecuteReader();
                reader.Read();
                Console.WriteLine(reader.GetString("userID") + " " + reader.GetString("userName") + " " + reader.GetString("userPassword") + " " + reader.GetString("sex") + " " + reader.GetString("userEmail"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                conn.Close();
            }
            Console.WriteLine("查看其他用户详情，请输入对应编号；返回 请按“b”");
            string op = Console.ReadLine();
            if (op == "b") SeeUser(adminname, password);
            else UserDetail(op, adminname, password);
        }
        static void ShowAllComment(string num, string adminname, string password)
        {
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=" + adminname + ";password=" + password + "";
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "select *  from comment,news_Comment where comment.commentID=news_Comment.commentID and newsID=" + num + ""; //我们自己按照查询条件组拼mysql 很麻烦
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                MySqlDataReader reader = command.ExecuteReader();
                Console.WriteLine("commentID  commentContent");
                Console.WriteLine("------------------------------------------------------------------");
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetInt32(0) + "          " + reader.GetString(1));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conn.Close();
            }
            Console.WriteLine("查看其他新闻的评论，请输入对应新闻编号(newsID)；删除评论 请按“d”  返回 请按“b”");
            string op = Console.ReadLine();
            if (op == "b") AdminOperate(adminname, password);
            else if (op == "d") DeleteComment(adminname, password);
            else ShowAllComment(op, adminname, password);
        }
        static void DeleteComment(string adminname, string password)
        {
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=" + adminname + ";password=" + password + "";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            Console.WriteLine("请输入要删除的评论的编号（commentID）");
            string commentID = Console.ReadLine();
            try
            {
                conn.Open();
                string sql = "delete from comment  where commentID='" + commentID + "'";
                MySqlCommand command = new MySqlCommand(sql, conn);
                command.ExecuteNonQuery();
                Console.WriteLine("已成功删除评论[{0}]", commentID);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                conn.Close();
            }
            SeeComment(adminname, password);
        }
        static int NewsCount(string adminname, string password)
        {
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=" + adminname + ";password=" + password + "";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库
            int commentID = 0;
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "select count(*)  from news";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                object reader = command.ExecuteScalar();
                commentID = Convert.ToInt32(reader.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            finally
            {
                conn.Close();
            }
            return commentID;
        }
        //发布新闻
        static void ReleaseNew(string adminname, string password)
        {
            Console.WriteLine("请按下列步骤操作：");
            Console.WriteLine("标题：");
            string newsTitle = Console.ReadLine();
            Console.WriteLine("内容：");
            string newsContent = Console.ReadLine();
            Console.WriteLine("简述：");
            string newsDesc = Console.ReadLine();
            Console.WriteLine("级别：");
            string newsRate = Console.ReadLine();
            int newsID = NewsCount(adminname, password);
            newsID++;
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=" + adminname + ";password=" + password + "";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "insert into news values(" + newsID + ",'" + newsTitle + "','" + newsContent + "','" + DateTime.Now + "','" + newsDesc + "',null," + newsRate + ")";
                //string sql = "insert into users(username,password,time) values('f','g','" + DateTime.Now + "')";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                command.ExecuteNonQuery();                                                    // MySqlDataReader reader = command.ExecuteReader();
                Console.WriteLine("成功发布新闻！");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                conn.Close();
            }
            Console.WriteLine("请问你接下来想进行什么操作？");
            Console.WriteLine("【1】继续发布新闻，【2】返回操作界面   【X】退出请点右上方的关闭按钮");
            string next = Console.ReadLine();
            if (next == "1") ReleaseNew(adminname, password);
            else if (next == "2") AdminOperate(adminname, password);

            Console.ReadKey();
        }
        static void InsertUC(int nCID, int userID, int commentID)
        {
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=root;password=123456";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "insert into users_Comment values(" + nCID + "," + userID + "," + commentID + ")";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                                                                    // MySqlDataReader reader = command.ExecuteReader();
                command.ExecuteNonQuery(); //返回的是数据库中受影响的数据的行数
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                conn.Close();
            }
        }
        static void InsertNC(int nCID, string newsID, int commentID)
        {
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=root;password=123456";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库
            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();
                string sql = "insert into news_Comment values(" + nCID + "," + newsID + "," + commentID + ")";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                                                                    // MySqlDataReader reader = command.ExecuteReader();
                command.ExecuteNonQuery(); //返回的是数据库中受影响的数据的行数
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                conn.Close();
            }
        }

        static void Read()
        {
            string connectStr = "server=127.0.0.1;port=3306;database=database_newssystem;user=user1;password=1123456";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库




            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();


                //string sql = "select id , password   from users";
                string sql = "select *  from news";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                MySqlDataReader reader = command.ExecuteReader();
                //reader.Read();  //调用一次相当于翻一页书


                //Console.WriteLine(reader[0].ToString() + reader[1].ToString() + reader[2].ToString());


                while (reader.Read()) //如果有数据 返回True 没数据返回false
                {
                    //Console.WriteLine(reader[0].ToString() + reader[1].ToString() /*+ reader[2].ToString()*/);


                    //Console.WriteLine( reader.GetInt32(0)+ " "+reader.GetString(1)+ " "+reader.GetString(2));
                    Console.WriteLine(reader.GetString("newsID") + " " + reader.GetString("newsContent"));
                }


                //command.ExecuteReader(); //执行一些查询
                //command.ExecuteNonQuery(); //插入删除
                //command.ExecuteScalar();//执行一些查询，返回一个单个值


                Console.WriteLine("已经建立连接");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                conn.Close();
            }


            Console.ReadKey();
        }//用于参考的源码
        static void Insert()
        {
            string connectStr = "server=127.0.0.1;port=3306;database=test;user=root;password=123456";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库




            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();


                string sql = "insert into users(username,password) values('f','g')";
                //string sql = "insert into users(username,password,time) values('f','g','" + DateTime.Now + "')";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                                                                    // MySqlDataReader reader = command.ExecuteReader();
                int result = command.ExecuteNonQuery(); //返回的是数据库中受影响的数据的行数
                Console.WriteLine(result);
                //Console.WriteLine("已经建立连接");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                conn.Close();
            }


            Console.ReadKey();
        }//用于参考的源码
        static void Update()
        {
            string connectStr = "server=127.0.0.1;port=3306;database=test;user=root;password=123456";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库




            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();



                string sql = "update  users set username='abc',password='222' where username='123456789'";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                                                                    // MySqlDataReader reader = command.ExecuteReader();
                int result = command.ExecuteNonQuery(); //返回的是数据库中受影响的数据的行数
                Console.WriteLine(result);
                Console.WriteLine("已经建立连接");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                conn.Close();
            }


            Console.ReadKey();
        }//用于参考的源码
        static void Delete()
        {
            string connectStr = "server=127.0.0.1;port=3306;database=test;user=root;password=123456";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库




            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();




                string sql = "delete from users  where username='abc'";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                                                                    // MySqlDataReader reader = command.ExecuteReader();
                int result = command.ExecuteNonQuery(); //返回的是数据库中受影响的数据的行数
                Console.WriteLine(result);
                Console.WriteLine("已经建立连接");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                conn.Close();
            }


            Console.ReadKey();
        }//用于参考的源码
        static void ReadUserCount()
        {
            string connectStr = "server=127.0.0.1;port=3306;database=test;user=root;password=123456";//填写用来连接数据库的IP地址，用户名密码 ，和连接的数据库




            MySqlConnection conn = new MySqlConnection(connectStr); // 建立连接通道  并没有跟数据库跟数据建立连接
            try
            {
                conn.Open();


                //string sql = "select id , password   from users";
                string sql = "select count(*)  from users";
                MySqlCommand command = new MySqlCommand(sql, conn); //如何向MySQL发起命令
                MySqlDataReader reader = command.ExecuteReader();
                reader.Read();
                int count = Convert.ToInt32(reader[0].ToString());


                Console.WriteLine(count);




                Console.WriteLine("已经建立连接");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                conn.Close();
            }


            Console.ReadKey();
        }//用于参考的源码

    }
}
