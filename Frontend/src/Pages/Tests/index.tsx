import { useNavigate } from 'react-router-dom'
import { session } from '../../session';

const Index = () => {
    const navigate = useNavigate();
    
    return (
    <div>
        Tests
        <button onClick={() => {
            session.logout();
            navigate('/login');
        }}>Logout</button>
    </div>
  )
}

export default Index